using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.PlayingPlakoto;

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
    public override Game ApplyToGame(Game g)
    {
        Game gameWithAllPlayParts = PlayParts
            .Aggregate(g, (aggregated, next) => next.ApplyToGame(aggregated));

        return gameWithAllPlayParts.State.GameOver()
            ? gameWithAllPlayParts
            : new Game(
                gameWithAllPlayParts.Board,
                GameState.PlayerMovedOrTiredOrBearedOff,
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
    public int SumOfCheckersCovered => PlayParts.Sum(pp =>
    ((pp as MovedOnBoardTurnPlayPart)?.Covering == true ? 1 : 0)
    - ((pp as MovedOnBoardTurnPlayPart)?.Uncovering == true ? 1 : 0)
    );
}

public abstract record MovedTurnPlayPart
{
    public int ValuePlayed { get; set; }
    public abstract Game ApplyToGame(Game portesGame);
    internal abstract string StringRepresentation();
    public abstract IReadOnlyList<(string key, string value)> GetPlayElements();

}
public record MovedOnBoardTurnPlayPart : MovedTurnPlayPart
{
    public int MovedFromPosition { get; set; }
    public int MovedToPosition { get; set; }
    public bool Covering { get; set; }
    public bool Uncovering { get; set; }

    public override Game ApplyToGame(Game g)
    {
        PlayerColour currentPlayer =
            g.State == GameState.PlayerWonRollForOrder
            ? g.StatePlayer.Value
            : g.StatePlayer.Value.GetNext(); // PortesGameState == PortesGameState.PlayerMovedOrBearedOffOrBoarded

        int currentPlayerDirection = currentPlayer == PlayerColour.White ? -1 : 1;

        bool checkerExistsOnSourcePosition = g.Board.GetTopChecker(MovedFromPosition) == new Checker(currentPlayer);
        if (!checkerExistsOnSourcePosition)
            throw new Exception("Cannot move: players checker not found on the source position");

        int destinationPosition = MovedFromPosition + ValuePlayed * currentPlayerDirection;

        if (!(destinationPosition < 24 && destinationPosition >= 0))
            throw new Exception("Cannot move: move outside the board");

        Checker destinationPositionTopChecker = g.Board.GetTopChecker(destinationPosition);
        int destinationPositionCheckerCount = g.Board.Points[destinationPosition]?.Checkers.Count ?? 0;
        if (!(destinationPositionTopChecker is null || destinationPositionTopChecker.Colour == currentPlayer || destinationPositionCheckerCount == 1))
            throw new Exception("Cannot move: oponent is ocupying the destination position");

        bool covering = destinationPositionTopChecker is not null && destinationPositionTopChecker.Colour != currentPlayer;
        if (covering != Covering)
            throw new Exception("Cannot move: it would result in the different covering flag");
        bool uncovering = g.Board.Points[MovedFromPosition].Checkers.Count == 2 && g.Board.Points[MovedFromPosition].Checkers[0].Colour != currentPlayer;
        if (uncovering != Uncovering)
            throw new Exception("Cannot move: it would result in the different uncovering flag");

        Point newSourcePoint = g.Board.Points[MovedFromPosition].WithRemovedCheckerFromTheTop();
        Point newDestinationPoint = g.Board.Points[destinationPosition].WithAddedOnTheTopChecker(currentPlayer);
        Board newBoard = new Board(g.Board.PointsWithReplacedPoints(new Dictionary<int, Point> { { MovedFromPosition, newSourcePoint }, { destinationPosition, newDestinationPoint } }));

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
        return $"Moves {ValuePlayed} from pos {MovedFromPosition} to pos {MovedToPosition}{(Covering ? " with covering" : string.Empty)}{(Uncovering ? " with uncovering" : string.Empty)}";
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

        int currentPlayerDirection = currentPlayer == PlayerColour.White ? -1 : 1;

        bool checkerExistsOnSourcePosition = g.Board.GetTopChecker(BearedOffFromPosition) == new Checker(currentPlayer);
        if (!checkerExistsOnSourcePosition)
            throw new Exception("Cannot bear off: players checker not found on the source position");

        if (!g.IsBearingPossible(currentPlayer))
            throw new Exception("Cannot bear off: player cannot bear off at the moment");

        int bearingPosition = currentPlayer == PlayerColour.White
            ? g.Board.ContainsPlayersCheckers(currentPlayer, ValuePlayed - 1, 5)
                ? ValuePlayed - 1
                : g.Board.Points.Select((p, i) => i).Where(i => i < ValuePlayed - 1).Reverse().First(i => g.Board.Points[i].ContainsPlayersCheckers(currentPlayer))
            : g.Board.ContainsPlayersCheckers(currentPlayer, 18, 24 - ValuePlayed)
                ? 24 - ValuePlayed
                : g.Board.Points.Select((p, i) => i).Where(i => i > 24 - ValuePlayed).First(i => g.Board.Points[i].ContainsPlayersCheckers(currentPlayer))
            ;
        if (BearedOffFromPosition != bearingPosition)
            throw new Exception("Cannot bear off: unexpected position to bear off from ");

        Point newSourcePoint = g.Board.Points[BearedOffFromPosition].WithRemovedCheckerFromTheTop();
        Board newBoard = new Board(g.Board.PointsWithReplacedPoints(new Dictionary<int, Point> { { BearedOffFromPosition, newSourcePoint } }));
        IReadOnlyDictionary<PlayerColour, IReadOnlyList<Checker>> newBearedOffCheckers = g.GetBearedOffCheckersWithChecker(new Checker(currentPlayer));
        bool newBoardContainsCurrentPlayerCheckers = newBoard.ContainsPlayersCheckers(currentPlayer);
        GameState newPortesGameState =
            newBoardContainsCurrentPlayerCheckers
            ? g.State // moved or rolled for order
            : g.BearedOffCheckers[currentPlayer.GetNext()].Count > 0
                ? GameState.PlayerWonSingle
                : GameState.PlayerWonDouble;
        PlayerColour newPortesGameStatePlayer =
            newBoardContainsCurrentPlayerCheckers
            ? g.StatePlayer.Value
            : currentPlayer;

        Game game = new Game(
            newBoard,
            newPortesGameState,
            newPortesGameStatePlayer,
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
