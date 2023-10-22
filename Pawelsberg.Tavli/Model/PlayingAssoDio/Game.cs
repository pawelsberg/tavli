using Pawelsberg.Tavli.Model.Common;
using Pawelsberg.Tavli.Model.EqualityComparers;
using Pawelsberg.Tavli.Model.Extensions;
using System.Text;

namespace Pawelsberg.Tavli.Model.PlayingAssoDio;

public record Game : GameBase
{
    public Board Board;
    public GameState State;
    public PlayerColour? StatePlayer;
    public IReadOnlyDictionary<PlayerColour, IReadOnlyList<Checker>> BearedOffCheckers;
    public IReadOnlyDictionary<PlayerColour, IReadOnlyList<Checker>> ToBeBoardedCheckers;

    private Game(Board board)
    {
        Board = board;
        State = GameState.Beginning;
        BearedOffCheckers = new Dictionary<PlayerColour, IReadOnlyList<Checker>>
            { { PlayerColour.White, new List<Checker>() }, {PlayerColour.Black, new List<Checker>() } };
        ToBeBoardedCheckers = new Dictionary<PlayerColour, IReadOnlyList<Checker>>
            {
                { PlayerColour.White, Enumerable.Repeat(new Checker(PlayerColour.White),15).ToList() },
                { PlayerColour.Black, Enumerable.Repeat(new Checker(PlayerColour.Black),15).ToList() }
            };
    }

    public override PlayerColour? GetStatePlayer() => StatePlayer;
    public override bool GetPlayerWonDouble() => State == GameState.PlayerWonDouble;

    public override PlayerColour? GetCurrentTurnPlayer()
    {
        PlayerColour? currentTurnPlayer =
            State == GameState.PlayerWonRollForOrder
            || State == GameState.PlayerMovedOrTriedOrBearedOffOrBoardedAfterAssoDioOrDouble
            ? StatePlayer.Value
            : State == GameState.PlayerMovedOrTriedOrBearedOffOrBoardedAfterTwoDifferent
                ? StatePlayer.Value.GetNext()
                : null;
        return currentTurnPlayer;
    }

    public Game(Board board, GameState state, PlayerColour? statePlayer, IReadOnlyDictionary<PlayerColour, IReadOnlyList<Checker>> bearedOffCheckers, IReadOnlyDictionary<PlayerColour, IReadOnlyList<Checker>> toBeBoardedCheckers)
    {
        foreach (PlayerColour playerColour in PlayerColourExtensions.AllPlayerColours())
        {
            if (bearedOffCheckers[playerColour].Any(c => c.Colour != playerColour))
                throw new Exception("Wrong colour in beared off checkers.");
            if (toBeBoardedCheckers[playerColour].Any(c => c.Colour != playerColour))
                throw new Exception("Wrong colour in to be boarded checkers.");
        }
        Board = board;
        State = state;
        StatePlayer = statePlayer;
        BearedOffCheckers = bearedOffCheckers;
        ToBeBoardedCheckers = toBeBoardedCheckers;
    }

    public static Game GetBeginning()
    {
        return new Game(Board.BeginningBoard);
    }

    private static IReadOnlyList<IReadOnlyList<int>> GetValueSetsFromDices(TurnRoll turnRoll)
    {
        if (turnRoll.Values.Count != 2)
            throw new Exception("Expected two dice roll");
        return turnRoll.Values[0] != turnRoll.Values[1]
            ? turnRoll.Values[0] == 1 && turnRoll.Values[1] == 2 || turnRoll.Values[0] == 2 && turnRoll.Values[1] == 1
                ? Enumerable.Range(1, 6).Select(dv => turnRoll.Values.Concat(Enumerable.Repeat(dv, 4)).ToList()).ToList()
                : new List<IReadOnlyList<int>> { turnRoll.Values }
            : new List<IReadOnlyList<int>> { Enumerable.Repeat(turnRoll.Values[0], 4).ToList() };
    }
    private static IReadOnlyList<IReadOnlyList<int>> GetAllPermutations(IReadOnlyList<int> values)
    {
        return values.Permute().Select(vp => vp.ToList()).Distinct(new ListOfIntsEqualityComparer()).ToList();
    }
    public bool IsBearingPossible(PlayerColour playerColour)
    {
        return !ToBeBoardedCheckers[playerColour].Any() && Board.IsBearingPossible(playerColour);
    }
    public IReadOnlyDictionary<PlayerColour, IReadOnlyList<Checker>> GetBearedOffCheckersWithChecker(Checker checker)
    {
        return
            BearedOffCheckers
                .Select(boc =>
                    boc.Key == checker.Colour
                    ? new KeyValuePair<PlayerColour, IReadOnlyList<Checker>>(
                            checker.Colour,
                            boc.Value.Concat(new List<Checker> { new Checker(checker.Colour) }).ToList())
                    : boc)
                .ToDictionary(boc => boc.Key, boc => boc.Value);
    }
    public IReadOnlyDictionary<PlayerColour, IReadOnlyList<Checker>> GetToBeBoardedCheckersWithChanges(Checker addedChecker = null, Checker removedChecker = null)
    {
        return
            ToBeBoardedCheckers
                .Select(tbbc =>
                    addedChecker != null && tbbc.Key == addedChecker.Colour
                    ? new KeyValuePair<PlayerColour, IReadOnlyList<Checker>>(
                            addedChecker.Colour,
                            tbbc.Value.Concat(new List<Checker> { new Checker(addedChecker.Colour) }).ToList())
                    : tbbc)
                .ToDictionary(tbbc => tbbc.Key, tbbc => tbbc.Value)
                .Select(tbbc =>
                    removedChecker != null && tbbc.Key == removedChecker.Colour
                    ? new KeyValuePair<PlayerColour, IReadOnlyList<Checker>>(
                            removedChecker.Colour,
                            tbbc.Value.Skip(1).ToList())
                    : tbbc)
                .ToDictionary(tbbc => tbbc.Key, tbbc => tbbc.Value)
                ;
    }

    public override IEnumerable<PlayerColour> GetPlayersRolling()
    {
        switch (State)
        {
            case GameState.Beginning:
            case GameState.PlayersDrawRollForOrder:
                return new List<PlayerColour> { PlayerColour.White, PlayerColour.Black };
            case GameState.PlayerWonRollForOrder:
            case GameState.PlayerMovedOrTriedOrBearedOffOrBoardedAfterAssoDioOrDouble:
                return new List<PlayerColour> { StatePlayer.Value };
            case GameState.PlayerMovedOrTriedOrBearedOffOrBoardedAfterTwoDifferent:
                return new List<PlayerColour> { StatePlayer.Value.GetNext() };
            case GameState.PlayerWonSingle:
            case GameState.PlayerWonDouble:
                return new List<PlayerColour>();
            default:
                throw new Exception("Unknown game state");
        }

    }
    public override IEnumerable<TurnRoll> GetAllTurnRolls()
    {
        switch (State)
        {
            case GameState.Beginning:
            case GameState.PlayersDrawRollForOrder:
                return TurnRoll.TwoPlayersRoll();
            case GameState.PlayerWonRollForOrder:
            case GameState.PlayerMovedOrTriedOrBearedOffOrBoardedAfterAssoDioOrDouble:
            case GameState.PlayerMovedOrTriedOrBearedOffOrBoardedAfterTwoDifferent:
                return TurnRoll.SinglePlayerRolls();
            case GameState.PlayerWonSingle:
            case GameState.PlayerWonDouble:
                return TurnRoll.NoRolling();
            default:
                throw new Exception("Unknown game state");
        }
    }
    public IEnumerable<TurnPlay> GetAllTurnPlays(TurnRoll turnRoll, bool removeRedundantPlays = true)
    {
        if (State == GameState.Beginning || State == GameState.PlayersDrawRollForOrder)
        {
            if (turnRoll.Values.Count != 2)
                throw new Exception("Need to have 2 rolls when rolling for order.");
            return new List<TurnPlay> { new RolledForOrderTurnPlay { WhitePlayerValueRolled = turnRoll.Values[0], BlackPlayerValueRolled = turnRoll.Values[1] } };
        }

        if (StatePlayer is null)
            throw new Exception($"Cannot establish player (gameStatePlayer:NULL)");
        if (State != GameState.PlayerWonRollForOrder && State != GameState.PlayerMovedOrTriedOrBearedOffOrBoardedAfterAssoDioOrDouble
            && State != GameState.PlayerMovedOrTriedOrBearedOffOrBoardedAfterTwoDifferent)
            throw new Exception($"Player cannot play at the moment (gameState:{State}, gameStatePlayer:{StatePlayer})");

        IReadOnlyList<IReadOnlyList<int>> valueSetsToUse = GetValueSetsFromDices(turnRoll); // values to act upon - after taking into account roll of double and asso dio

        PlayerColour currentPlayer = GetCurrentTurnPlayer().Value;
        IReadOnlyList<int> turnRollValues = turnRoll.Values;
        bool doubleOrAssoDioRoll = turnRollValues.Count == 2 && (
            turnRollValues[0] == turnRollValues[1]
            || turnRollValues[0] == 1 && turnRollValues[1] == 2
            || turnRollValues[0] == 2 && turnRollValues[1] == 1);

        List<MovedTurnPlay> allMovedTurnPlays = valueSetsToUse.SelectMany(valuesToUse =>
         {
             var turnPlayGamesWithAllPossibleValuesPlayed =
                 GetAllPermutations(valuesToUse)
                 .SelectMany(valuesToUsePermutation => GetTurnPlaysWithAllPosibleValuesPlayed(valuesToUsePermutation, MovedTurnPlay.NoMove(currentPlayer, doubleOrAssoDioRoll), doubleOrAssoDioRoll))
                 .Select(mtp => new { mtp, g = mtp.ApplyToGame(this) })
                 .ToList();

             IReadOnlyList<MovedTurnPlay> turnPlaysWithAllPossibleValuesPlayedFiltered = removeRedundantPlays
                 ? turnPlayGamesWithAllPossibleValuesPlayed
                 .GroupBy(mtpg => mtpg.g, new GameEqualityComparer())
                 .Select(gmtpg => gmtpg.First().mtp).ToList()
                 : turnPlayGamesWithAllPossibleValuesPlayed.Select(mtpg => mtpg.mtp).ToList();

             if (turnPlaysWithAllPossibleValuesPlayedFiltered.Any())
             { // can play - only possibilities with highest number of field moves allowed
                 int maxPlayParts = turnPlaysWithAllPossibleValuesPlayedFiltered.Max(walvp => walvp.PlayParts.Count);
                 IEnumerable<MovedTurnPlay> turnPlaysWithMaxPlayParts =
                     turnPlaysWithAllPossibleValuesPlayedFiltered
                     .Where(wapvp => wapvp.PlayParts.Count == maxPlayParts);

                 return turnPlaysWithMaxPlayParts
                     .Distinct(new MovedTurnPlayEqualityComparer());
             }
             else
             { // no possible moves - next player rolls
                 return new List<MovedTurnPlay> { MovedTurnPlay.NoMove(currentPlayer, doubleOrAssoDioRoll) };
             }


         }
        ).ToList();
        int maxPlayParts = allMovedTurnPlays.Max(mtp => mtp.PlayParts.Count);
        return allMovedTurnPlays.Where(mtp => mtp.PlayParts.Count == maxPlayParts);

    }

    private IEnumerable<MovedTurnPlay> GetTurnPlaysWithAllPosibleValuesPlayed(IReadOnlyList<int> valuesToUse, MovedTurnPlay turnPlay, bool doubleOrAssoDioRoll)
    {
        if (!valuesToUse.Any()) // recursion stop condition
            return new List<MovedTurnPlay>();

        int valueToUse = valuesToUse.First();
        IEnumerable<MovedTurnPlayPart> withValueToUseTurnPlayParts = GetTurnPlaysWithPlayerSingleCheckerPlayPart(valueToUse, doubleOrAssoDioRoll);
        IEnumerable<MovedTurnPlay> withValueToUseTurnPlays = withValueToUseTurnPlayParts
                .Select(wvtutpp => turnPlay.WithPlayPart(wvtutpp));

        if (valuesToUse.Count == 1) // if last players move
            return withValueToUseTurnPlays;
        else
        {
            IReadOnlyList<int> restValuesToUse = valuesToUse.Skip(1).ToList();
            return withValueToUseTurnPlayParts.Select(wvtutpp => new { Game = wvtutpp.ApplyToGame(this), TurnPlayPart = wvtutpp })
                .Where(gwvtutpp => !gwvtutpp.Game.State.GameOver())
                .SelectMany(gwvtutpp => gwvtutpp.Game.GetTurnPlaysWithAllPosibleValuesPlayed(restValuesToUse, turnPlay.WithPlayPart(gwvtutpp.TurnPlayPart), doubleOrAssoDioRoll))
                .Concat(withValueToUseTurnPlays);
        }

    }
    private IEnumerable<MovedTurnPlayPart> GetTurnPlaysWithPlayerSingleCheckerPlayPart(int valueToUse, bool doubleOrAssoDioRoll)
    {
        PlayerColour currentPlayer = GetCurrentTurnPlayer().Value;

        int currentPlayerDirection = currentPlayer == PlayerColour.White ? -1 : 1;

        bool anyCheckersToBeBoarded = ToBeBoardedCheckers[currentPlayer].Any();
        if (!anyCheckersToBeBoarded)
        {
            (int startSourcePosition, int endSourcePosition) = Board.GetSourceZone(currentPlayer);
            // moving or bearing
            foreach (int sourcePosition in Enumerable.Range(startSourcePosition, endSourcePosition - startSourcePosition + 1))
            {
                bool checkerExistsOnSourcePosition = Board.GetTopChecker(sourcePosition) == new Checker(currentPlayer);
                if (checkerExistsOnSourcePosition)
                {
                    int destinationPosition = sourcePosition + valueToUse * currentPlayerDirection;

                    if (IsBearingPossible(currentPlayer))
                    { // bear it if possible
                        int bearingPosition = currentPlayer == PlayerColour.White
                            ? valueToUse - 1
                            : 24 - valueToUse;

                        if (sourcePosition == bearingPosition)
                        {
                            BearedOffTurnPlayPart bearOffTurnPlayPart = new BearedOffTurnPlayPart
                            {
                                ValuePlayed = valueToUse,
                                BearedOffFromPosition = sourcePosition
                            };
                            yield return bearOffTurnPlayPart;
                        }
                    }
                    else if (destinationPosition < 24 && destinationPosition >= 0)
                    { // move it if possible
                        Checker destinationPositionTopChecker = Board.GetTopChecker(destinationPosition);
                        int destinationPositionCheckerCount = Board.Points[destinationPosition]?.Checkers.Count ?? 0;
                        if (destinationPositionTopChecker is null || destinationPositionTopChecker.Colour == currentPlayer || destinationPositionCheckerCount == 1)
                        { // if destination available
                            bool beating = destinationPositionTopChecker is not null && destinationPositionTopChecker.Colour != currentPlayer;
                            MovedOnBoardTurnPlayPart moveTurnPlayPart = new MovedOnBoardTurnPlayPart
                            {
                                Beating = beating,
                                ValuePlayed = valueToUse,
                                MovedFromPosition = sourcePosition,
                                MovedToPosition = destinationPosition
                            };
                            yield return moveTurnPlayPart;
                        }
                    }
                }
            }

        }
        else if (doubleOrAssoDioRoll || Board.CheckersInTheFirstQuarter(currentPlayer))
        {
            // board it if possible
            Checker boardingChecker = ToBeBoardedCheckers[currentPlayer].First();
            int boardingStartPosition = currentPlayer == PlayerColour.White ? 23 : 0;
            int boardingPosition = boardingStartPosition + (valueToUse - 1) * currentPlayerDirection;
            int boardingPositionCheckerCount = Board.Points[boardingPosition]?.Checkers.Count ?? 0;
            Checker boardingPositionTopChecker = Board.Points[boardingPosition].TopChecker;
            if (boardingPositionTopChecker is null || boardingPositionTopChecker.Colour == currentPlayer || boardingPositionCheckerCount == 1)
            { // board
                bool beating = boardingPositionTopChecker is not null && boardingPositionTopChecker.Colour != currentPlayer;
                BoardedTurnPlayPart boardTurnPlayPart = new BoardedTurnPlayPart
                {
                    Beating = beating,
                    ValuePlayed = valueToUse,
                    BoardingPosition = boardingPosition
                };
                yield return boardTurnPlayPart;
            }
        }
    }

    public int GetPips(PlayerColour player)
    {
        if (State.GameOver() && StatePlayer == player)
            return 0;

        int playerDirection = player == PlayerColour.White ? -1 : 1;
        int playerHomesEndPosition = player == PlayerColour.White ? 0 : 23;

        return ToBeBoardedCheckers[player].Count * 26
            + Board.Points.Select((p, i) => (i, p)).Sum(ip => ip.p.Checkers.Where(c => c.Colour == player).Count() * ((playerHomesEndPosition - ip.i) * playerDirection + 1));
    }
    public int SingleEndangeredCheckers(PlayerColour playerColour)
    {
        PlayerColour oponentColour = playerColour.GetNext();
        int? startBeatingArea = playerColour == PlayerColour.White
            ? ToBeBoardedCheckers[oponentColour].Any()
                ? 0
                : Board.Points.Select((p, i) => new { p, i }).FirstOrDefault(pi => pi.p.Checkers?.FirstOrDefault()?.Colour == oponentColour)?.i + 1
            : 6;
        int? endBeatingArea = playerColour == PlayerColour.White
            ? 17
            : ToBeBoardedCheckers[oponentColour].Any()
                ? 23
                : Board.Points.Select((p, i) => new { p, i }).Reverse().FirstOrDefault(pi => pi.p.Checkers?.FirstOrDefault()?.Colour == oponentColour)?.i - 1;
        if (!startBeatingArea.HasValue || !endBeatingArea.HasValue)
            return 0;
        return Board.Points
            .Skip(startBeatingArea.Value)
            .Take(endBeatingArea.Value - startBeatingArea.Value + 1)
            .Count(p => p.Checkers.Count == 1 && p.Checkers.FirstOrDefault()?.Colour == playerColour);
    }
    public int Portes(PlayerColour playerColour)
    {
        PlayerColour oponentColour = playerColour.GetNext();
        int? startBeatingArea = playerColour == PlayerColour.White
            ? ToBeBoardedCheckers[oponentColour].Any()
                ? 0
                : Board.Points.Select((p, i) => new { p, i }).FirstOrDefault(pi => pi.p.Checkers?.FirstOrDefault()?.Colour == oponentColour)?.i + 1
            : 6;
        int? endBeatingArea = playerColour == PlayerColour.White
            ? 17
            : ToBeBoardedCheckers[oponentColour].Any()
                ? 23
                : Board.Points.Select((p, i) => new { p, i }).Reverse().FirstOrDefault(pi => pi.p.Checkers?.FirstOrDefault()?.Colour == oponentColour)?.i - 1;
        if (!startBeatingArea.HasValue || !endBeatingArea.HasValue)
            return 0;
        return Board.Points
            .Skip(startBeatingArea.Value)
            .Take(endBeatingArea.Value - startBeatingArea.Value + 1)
            .Count(p => p.Checkers.Count == 2 && p.Checkers.FirstOrDefault()?.Colour == playerColour);
    }


    public override string StringRepresentation()
    {
        int bigestPointLength = Board.Points.Max(p => p?.Checkers.Count ?? 0);
        int pointLength = Math.Max(bigestPointLength, 6);
        // todo - remove mutability - (sb)
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"State:{State} Player:{StatePlayer} White player pips:{GetPips(PlayerColour.White)} Black player pips:{GetPips(PlayerColour.Black)}");
        sb.AppendLine("  0   1   2   3   4   5       6   7   8   9  10  11");
        sb.AppendLine("╔═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╗");

        foreach (int topLineIndex in Enumerable.Range(0, pointLength))
            sb.AppendLine(TopLineStringRepresentation(topLineIndex));

        sb.AppendLine("╠═══╬═══╬═══╬═══╬═══╬═══╬BAR╬═══╬═══╬═══╬═══╬═══╬═══╬OFF╣");

        foreach (int bottomLineIndex in Enumerable.Range(0, pointLength).Reverse())
            sb.AppendLine(BottomLineStringRepresentation(bottomLineIndex, pointLength));

        sb.AppendLine("╚═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╝");
        sb.AppendLine(" 23  22  21  20  19  18      17  16  15  14  13  12");

        return sb.ToString();
    }

    public override bool GameOver() => State.GameOver();

    private string BottomLineStringRepresentation(int bottomLineIndex, int pointLength)
    {
        // TODO - remove mutability (sb)
        StringBuilder sb = new StringBuilder();
        sb.Append(BoardSixStringRepresentation(bottomLineIndex, 18, true));
        if (bottomLineIndex == pointLength - 2)
        {
            int whiteCheckersToBeBoarded = ToBeBoardedCheckers[PlayerColour.White].Count;
            sb.Append(
                whiteCheckersToBeBoarded > 0
                ? $"{whiteCheckersToBeBoarded,2} "
                : "   "
                );
        }
        else if (bottomLineIndex == pointLength - 3)
        {
            int whiteCheckersToBeBoarded = ToBeBoardedCheckers[PlayerColour.White].Count;
            sb.Append(
                whiteCheckersToBeBoarded > 0
                ? $" {PlayerColour.White.ToCharacter()} "
                : "   "
                );
        }
        else sb.Append("   ");
        sb.Append(BoardSixStringRepresentation(bottomLineIndex, 12, true));
        if (bottomLineIndex == 1)
        {
            int blackBearedOffCheckers = BearedOffCheckers[PlayerColour.Black].Count;
            sb.Append(
                blackBearedOffCheckers > 0
                    ? $"{blackBearedOffCheckers,2} "
                    : "   "
            );
        }
        else if (bottomLineIndex == 2)
        {
            int blackBearedOffCheckers = BearedOffCheckers[PlayerColour.Black].Count;
            sb.Append(
                blackBearedOffCheckers > 0
                    ? $" {PlayerColour.Black.ToCharacter()} "
                    : "   "
            );
        }

        else sb.Append("   ");
        sb.Append("║");
        return sb.ToString();
    }
    private string BoardSixStringRepresentation(int depth, int start, bool reverse)
    {
        // TODO - remove mutability (sb)
        StringBuilder sb = new StringBuilder();
        sb.Append("║");
        IEnumerable<int> positionUnsortedRange = Enumerable.Range(start, 6);
        IEnumerable<int> positionSortedRange = reverse ? positionUnsortedRange.Reverse() : positionUnsortedRange;
        int end = positionSortedRange.Last();
        foreach (int position in positionSortedRange)
        {
            Point point = Board.Points[position];
            int pointCheckerCount = point?.Checkers?.Count ?? 0;
            Checker checker = pointCheckerCount >= depth + 1 ?
                point.Checkers[depth]
                : null;
            sb.Append(" ");
            sb.Append(checker is null ? " " : checker.Colour.ToCharacter());
            sb.Append(" ");
            sb.Append(position == end ? "║" : "│");
        }
        return sb.ToString();
    }
    private string TopLineStringRepresentation(int topLineIndex)
    {
        // TODO - remove mutability (sb)
        StringBuilder sb = new StringBuilder();
        sb.Append(BoardSixStringRepresentation(topLineIndex, 0, false));
        if (topLineIndex == 1)
        {
            int blackCheckersToBeBoarded = ToBeBoardedCheckers[PlayerColour.Black].Count;
            sb.Append(
                blackCheckersToBeBoarded > 0
                ? $"{blackCheckersToBeBoarded,2} "
                : "   "
                );
        }
        else if (topLineIndex == 2)
        {
            int blackCheckersToBeBoarded = ToBeBoardedCheckers[PlayerColour.Black].Count;
            sb.Append(
                blackCheckersToBeBoarded > 0
                ? $" {PlayerColour.Black.ToCharacter()} "
                : "   "
                );
        }
        else sb.Append("   ");
        sb.Append(BoardSixStringRepresentation(topLineIndex, 6, false));
        if (topLineIndex == 1)
        {
            int whiteBearedOffCheckers = BearedOffCheckers[PlayerColour.White].Count;
            sb.Append(
                whiteBearedOffCheckers > 0
                    ? $"{whiteBearedOffCheckers,2} "
                    : "   "
            );
        }
        else if (topLineIndex == 2)
        {
            int whiteBearedOffCheckers = BearedOffCheckers[PlayerColour.White].Count;
            sb.Append(
                whiteBearedOffCheckers > 0
                    ? $" {PlayerColour.White.ToCharacter()} "
                    : "   "
            );
        }
        else sb.Append("   ");
        sb.Append("║");
        return sb.ToString();
    }

    public static Game ParseFromStringRepresentation(string text)
    {
        //ParsableText parsableText = new ParsableText { Text = text }
        //    .Read("State:")
        //    .Read(out GameState gameState)
        //    .Read(" Player:")
        //    .Read(out PlayerColour playerColour)
        //    .Read(" White player pips:")
        //    .ReadInt(out int whitePlayerPips)
        //    .Read(" Black player pips:")
        //    .ReadInt(out int blackPlayerPips)
        //    .Read("\r\n 11  10   9   8   7   6       5   4   3   2   1   0\r\n╔═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╦═══╗\r\n")
        //    .ReadBoardLine(out List<PlayerColour?> topFirstLeftBoardSix, out List<PlayerColour?> topFirstRightBoardSix)
        //    .ReadBoardLineWithNumbers(out List<PlayerColour?> topSecondLeftBoardSix, out int? blackCheckersToBeBoarded, out List<PlayerColour?> topSecondRightBoardSix, out int? whiteCheckersBearedOff)
        //    .ReadBoardLineWithPlayerColours(out List<PlayerColour?> topThirdLeftBoardSix, out PlayerColour? blackCheckersToBeBoardedPlayerColour, out List<PlayerColour?> topThirdRightBoardSix, out PlayerColour? whiteCheckersBearedOffPlayerColour)
        //    .WhileCharacterReadBoardLines('╠', out List<(List<PlayerColour?>, List<PlayerColour?>)> topLines)
        //    .Read("╠═══╬═══╬═══╬═══╬═══╬═══╬BAR╬═══╬═══╬═══╬═══╬═══╬═══╬OFF╣\r\n")
        //    .ReadBoardLine(out List<PlayerColour?> bottomFirstLeftBoardSix, out List<PlayerColour?> bottomFirstRightBoardSix)
        //    .ReadBoardLineWithNumbers(out List<PlayerColour?> bottomSecondLeftBoardSix, out int? whiteCheckersToBeBoarded, out List<PlayerColour?> bottomSecondRightBoardSix, out int? blackCheckersBearedOff)
        //    .ReadBoardLineWithPlayerColours(out List<PlayerColour?> bottomThirdLeftBoardSix, out PlayerColour? whiteCheckersToBeBoardedPlayerColour, out List<PlayerColour?> bottomThirdRightBoardSix, out PlayerColour? blackCheckersBearedOffPlayerColour)
        //    .WhileCharacterReadBoardLines('╚', out List<(List<PlayerColour?>, List<PlayerColour?>)> bottomLines)
        //    .Read("╚═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╩═══╝\r\n")
        //    .Read(" 12  13  14  15  16  17      18  19  20  21  22  23")
        //    ;

        //IReadOnlyDictionary<PlayerColour, IReadOnlyList<Checker>> bearedOffCheckers = new Dictionary<PlayerColour, IReadOnlyList<Checker>> {
        //    {PlayerColour.White, Enumerable.Repeat(new Checker(PlayerColour.White),whiteCheckersBearedOff?? 0).ToList() },
        //    {PlayerColour.Black, Enumerable.Repeat(new Checker(PlayerColour.Black),blackCheckersBearedOff?? 0).ToList() }
        //};

        //IReadOnlyDictionary<PlayerColour, IReadOnlyList<Checker>> toBeBoardedCheckers = new Dictionary<PlayerColour, IReadOnlyList<Checker>> {
        //    {PlayerColour.White, Enumerable.Repeat(new Checker(PlayerColour.White),whiteCheckersToBeBoarded?? 0).ToList() },
        //    {PlayerColour.Black, Enumerable.Repeat(new Checker(PlayerColour.Black),blackCheckersToBeBoarded?? 0).ToList() }
        //};
        //List<List<PlayerColour?>> topRightBoard = new List<List<PlayerColour?>> { topFirstRightBoardSix, topSecondRightBoardSix, topThirdRightBoardSix }.Concat(topLines.Select(l => l.Item2)).ToList();
        //List<List<PlayerColour?>> topLeftBoard = new List<List<PlayerColour?>> { topFirstLeftBoardSix, topSecondLeftBoardSix, topThirdLeftBoardSix }.Concat(topLines.Select(l => l.Item1)).ToList();
        //List<List<PlayerColour?>> bottomLeftBoard = new List<List<PlayerColour?>> { bottomFirstLeftBoardSix, bottomSecondLeftBoardSix, bottomThirdLeftBoardSix }.Concat(bottomLines.Select(l => l.Item1)).ToList();
        //List<List<PlayerColour?>> bottomRightBoard = new List<List<PlayerColour?>> { bottomFirstRightBoardSix, bottomSecondRightBoardSix, bottomThirdRightBoardSix }.Concat(bottomLines.Select(l => l.Item2)).ToList();

        //Board board = new Board(Board.CreatePointsFromQuaters(topRightBoard, topLeftBoard, bottomLeftBoard, bottomRightBoard));
        //// TODO Checks

        //Game result = new Game(board, gameState, playerColour, bearedOffCheckers, toBeBoardedCheckers);
        //// TODO - check pips

        //return result;
        throw new NotImplementedException();
    }
}
