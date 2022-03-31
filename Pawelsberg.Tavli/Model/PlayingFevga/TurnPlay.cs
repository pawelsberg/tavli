using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.PlayingFevga;

public abstract record TurnPlay : TurnPlayBase
{
    public override GameBase ApplyToGame(GameBase game)
    {
        if (game is null)
            throw new Exception("Game cannot be null");
        if (game is not Game)
            throw new Exception("Cannot apply this turn play to this type of game");
        return ApplyToGame(game as Game);
    }
    public abstract Game ApplyToGame(Game g);
}

public record RolledForOrderTurnPlay : TurnPlay
{
    public int WhitePlayerValueRolled { get; set; }
    public int BlackPlayerValueRolled { get; set; }

    public override Game ApplyToGame(Game g)
    {
        if (g.State != GameState.Beginning && g.State != GameState.PlayersDrawRollForOrder)
            throw new Exception("Cannot roll for order at that stage of the game");

        GameState newGameState = WhitePlayerValueRolled == BlackPlayerValueRolled
            ? GameState.PlayersDrawRollForOrder
            : GameState.PlayerWonRollForOrder;
        PlayerColour? newGameStatePlayer = newGameState == GameState.PlayerWonRollForOrder
            ? WhitePlayerValueRolled > BlackPlayerValueRolled
                ? PlayerColour.White
                : PlayerColour.Black
            : null;
        return new Game(g.Board, newGameState, newGameStatePlayer, g.BearedOffCheckers);
    }

    public override IReadOnlyList<(string key, string value)> GetPlayElements()
    {
        return new List<(string key, string value)> { ("Roll", "r") };
    }

    public override string StringRepresentation()
    {
        return $"Players roll for order. White:{WhitePlayerValueRolled}, Black:{BlackPlayerValueRolled}";
    }
}

public record MovedTurnPlay : TurnPlay
{
    public IReadOnlyList<MovedTurnPlayPart> PlayParts;
    public PlayerColour PlayedByPlayer { get; set; }
    public MovedTurnPlay() { }
    private MovedTurnPlay(PlayerColour playedByPlayer)
    {
        PlayParts = new List<MovedTurnPlayPart>();
        PlayedByPlayer = playedByPlayer;
    }
    public static MovedTurnPlay NoMove(PlayerColour playedByPlayer)
    {
        return new MovedTurnPlay(playedByPlayer);
    }
    public MovedTurnPlay WithPlayPart(MovedTurnPlayPart playPart)
    {
        return new MovedTurnPlay(PlayedByPlayer) { PlayParts = PlayParts.Concat(new List<MovedTurnPlayPart> { playPart }).ToList() };
    }
    public bool InvalidBecauseOfAllHomePositionsTaken(Game g)
    {
        return PlayParts
            .Aggregate(g, (aggregated, next) => next.ApplyToGame(aggregated))
            .Board
            .AllHomePositionsTaken(PlayedByPlayer);
    }
    public override Game ApplyToGame(Game g)
    {
        Game gameWithAllPlayParts = PlayParts
            .Aggregate(g, (aggregated, next) => next.ApplyToGame(aggregated));

        if (gameWithAllPlayParts.Board.AllHomePositionsTaken(PlayedByPlayer))
            throw new Exception("Cannot move: cannot cover all home positions");

        return gameWithAllPlayParts.State.GameOver()
            ? gameWithAllPlayParts
            : new Game(
                gameWithAllPlayParts.Board,
                GameState.PlayerMovedOrTriedOrBearedOff,
                PlayedByPlayer,
                gameWithAllPlayParts.BearedOffCheckers);
    }

    public override string StringRepresentation()
    {
        return $"{PlayParts.Count} moves: {string.Join(", ", PlayParts.Select(pp => pp.StringRepresentation()))}";
    }

    public override IReadOnlyList<(string key, string value)> GetPlayElements()
    {
        return PlayParts.SelectMany(pp => pp.GetPlayElements()).ToList();
    }

    public int SumOfValuesPlayed => PlayParts.Sum(pp => pp.ValuePlayed);
}

public abstract record MovedTurnPlayPart
{
    public int ValuePlayed { get; set; }
    public abstract Game ApplyToGame(Game game);
    internal abstract string StringRepresentation();
    public abstract IReadOnlyList<(string key, string value)> GetPlayElements();
}
public record MovedOnBoardTurnPlayPart : MovedTurnPlayPart
{
    public int MovedFromPosition { get; set; }
    public int MovedToPosition { get; set; }

    public override Game ApplyToGame(Game g)
    {
        PlayerColour currentPlayer =
            g.State == GameState.PlayerWonRollForOrder
            ? g.StatePlayer.Value
            : g.StatePlayer.Value.GetNext();

        bool checkerExistsOnSourcePosition = g.Board.GetTopChecker(MovedFromPosition) == new Checker(currentPlayer);
        if (!checkerExistsOnSourcePosition)
            throw new Exception("Cannot move: players checker not found on the source position");

        bool wouldMoveBeyondBearingOffPosition = currentPlayer == PlayerColour.White
            ? MovedFromPosition + ValuePlayed >= 24
            : MovedFromPosition + ValuePlayed >= 12 && MovedFromPosition < 12;

        if (wouldMoveBeyondBearingOffPosition)
            throw new Exception("Cannot move: move outside the board");

        bool allCheckersOnTheFirstHalf = g.Board.AreAllCheckersOnTheFirstHalf(currentPlayer);
        int furthestCheckerPosition = g.Board.Points.Select((p, i) => (p, i)).Last(pi => pi.p.ContainsPlayersCheckers(currentPlayer)).i;
        if (allCheckersOnTheFirstHalf && MovedFromPosition != furthestCheckerPosition)
            throw new Exception("Cannot move: all checkers are on the first half of the board and not a move of the furthest checker");

        Checker destinationPositionTopChecker = g.Board.GetTopChecker(MovedToPosition);

        if (!(destinationPositionTopChecker is null || destinationPositionTopChecker.Colour == currentPlayer))
            throw new Exception("Cannot move: oponent is ocupying the destination position");

        Point sourcePoint = g.Board.Points[MovedFromPosition];
        Point destinationPoint = g.Board.Points[MovedToPosition];
        Point newSourcePoint = sourcePoint.WithRemovedCheckerFromTheTop();
        Point newDestinationPoint = destinationPoint.WithAddedOnTheTopChecker(currentPlayer);
        Board newBoard = new Board(g.Board.PointsWithReplacedPoints(new Dictionary<int, Point> { { MovedFromPosition, newSourcePoint }, { MovedToPosition, newDestinationPoint } }));

        Game game = new Game(
            newBoard,
            g.State,
            g.StatePlayer,
            g.BearedOffCheckers);
        return game;
    }

    public override IReadOnlyList<(string key, string value)> GetPlayElements()
    {
        return new List<(string key, string value)> { ("From", MovedFromPosition.ToString()), ("To", MovedToPosition.ToString()) };
    }

    internal override string StringRepresentation()
    {
        return $"Moves {ValuePlayed} from pos {MovedFromPosition} to pos {MovedToPosition}";
    }
}
public record BearedOffTurnPlayPart : MovedTurnPlayPart
{
    public int BearedOffFromPosition { get; set; }

    public override Game ApplyToGame(Game g)
    {
        PlayerColour currentPlayer =
            g.State == GameState.PlayerWonRollForOrder
            ? g.StatePlayer.Value
            : g.StatePlayer.Value.GetNext();

        bool checkerExistsOnSourcePosition = g.Board.GetTopChecker(BearedOffFromPosition) == new Checker(currentPlayer);
        if (!checkerExistsOnSourcePosition)
            throw new Exception("Cannot bear off: players checker not found on the source position");

        if (!g.IsBearingPossible(currentPlayer))
            throw new Exception("Cannot bear off: player cannot bear off at the moment");

        int bearingPosition = currentPlayer == PlayerColour.White
            ? g.Board.ContainsPlayersCheckers(currentPlayer, 18, 24 - ValuePlayed)
                ? 24 - ValuePlayed
                : g.Board.Points.Select((p, i) => (p, i)).Where(pi => pi.i > 24 - ValuePlayed).First(pi => pi.p.ContainsPlayersCheckers(currentPlayer)).i
            : g.Board.ContainsPlayersCheckers(currentPlayer, 6, 12 - ValuePlayed)
                ? 12 - ValuePlayed
                : g.Board.Points.Select((p, i) => (p, i)).Where(pi => pi.i > 12 - ValuePlayed && pi.i < 12).First(pi => pi.p.ContainsPlayersCheckers(currentPlayer)).i
            ;
        if (BearedOffFromPosition != bearingPosition)
            throw new Exception("Cannot bear off: unexpected position to bear off from ");

        Point newSourcePoint = g.Board.Points[BearedOffFromPosition].WithRemovedCheckerFromTheTop();
        Board newBoard = new Board(g.Board.PointsWithReplacedPoints(new Dictionary<int, Point> { { BearedOffFromPosition, newSourcePoint } }));
        IReadOnlyDictionary<PlayerColour, IReadOnlyList<Checker>> newBearedOffCheckers = g.GetBearedOffCheckersWithChecker(new Checker(currentPlayer));
        bool newBoardContainsCurrentPlayerCheckers = newBoard.ContainsPlayersCheckers(currentPlayer);
        GameState newGameState =
            newBoardContainsCurrentPlayerCheckers
            ? g.State // moved or rolled for order
            : g.BearedOffCheckers[currentPlayer.GetNext()].Count > 0
                ? GameState.PlayerWonSingle
                : GameState.PlayerWonDouble;
        PlayerColour newGameStatePlayer =
            newBoardContainsCurrentPlayerCheckers
            ? g.StatePlayer.Value
            : currentPlayer;

        Game game = new Game(
            newBoard,
            newGameState,
            newGameStatePlayer,
            newBearedOffCheckers);
        return game;
    }

    public override IReadOnlyList<(string key, string value)> GetPlayElements()
    {
        return new List<(string key, string value)> { ("From", BearedOffFromPosition.ToString()), ("To", "b") };
    }

    internal override string StringRepresentation()
    {
        return $"Plays {ValuePlayed} and bears off checker from the position {BearedOffFromPosition}";
    }
}

