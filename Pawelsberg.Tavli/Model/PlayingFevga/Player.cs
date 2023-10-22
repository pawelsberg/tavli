using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.PlayingFevga;

public abstract record Player : PlayerBase
{
    public abstract TurnPlay ChooseTurnPlay(Game game, TurnRoll roll);

    public override TurnPlayBase ChooseTurnPlay(GameBase game, TurnRoll roll) 
        => ChooseTurnPlay((Game)game, roll);
}

public record QuickPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        return game.GetAllTurnPlays(roll).FirstOrDefault();
    }
}

public record AskPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        List<TurnPlay> possibleTurnPlays = game.GetAllTurnPlays(roll).ToList();

        if (!possibleTurnPlays.Any())
            return null;

        Console.WriteLine("Possible plays:");
        foreach ((TurnPlay play, int index) in Enumerable.Range(1, possibleTurnPlays.Count).Select(i => (possibleTurnPlays[i - 1], i)))
            Console.WriteLine($"{index} - {play.StringRepresentation()}");

        Console.Write("Play>");
        string playText = Console.ReadLine();

        if (int.TryParse(playText, out int playInt))
        {
            if (playInt < 1 || playInt > possibleTurnPlays.Count)
                throw new Exception("Wrong play");
            return possibleTurnPlays[playInt - 1];
        }
        else
            throw new Exception("Wrong play");
    }
}

public record BestCoverageInTwoRoundsPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        if (game.State != GameState.PlayerWonRollForOrder && game.State != GameState.PlayerMovedOrTriedOrBearedOff)
            return game.GetAllTurnPlays(roll).FirstOrDefault();
        PlayerColour currentPlayer =
            game.State == GameState.PlayerWonRollForOrder
            ? game.StatePlayer.Value
            : game.StatePlayer.Value.GetNext();

        List<TurnPlay> possibleTurnPlays = new List<TurnPlay>(game.GetAllTurnPlays(roll));

        var turnPlaysGames = possibleTurnPlays
            .Select(tp => new { tp, g = tp.ApplyToGame(game) });

        var winningTurnPlay = turnPlaysGames
            .FirstOrDefault(tpg => tpg.g.State.GameOver())?.tp;
        if (winningTurnPlay != null)
            return winningTurnPlay;

        var turnPlaysSorted = turnPlaysGames
            .Select(tpg => new { tpg.tp, tpg.g, p = tpg.g.GetPlayersRolling().Single() })
            .SelectMany(tpgp => tpgp.g.GetAllTurnRolls(), (tpgp, r) => new { tpgp.tp, tpgp.g, tpgp.p, r })
            .SelectMany(tpgpr => tpgpr.g.GetAllTurnPlays(tpgpr.r), (tpgpr, ntp) => new { tpgpr.tp, tpgpr.g, tpgpr.p, tpgpr.r, ntp })
            .Select(tpgprntp =>
            {
                Game ng = tpgprntp.ntp.ApplyToGame(tpgprntp.g);
                MovedTurnPlay movedTurnPlay = tpgprntp.tp as MovedTurnPlay;
                MovedTurnPlay nextMovedTurnPlay = tpgprntp.ntp as MovedTurnPlay;
                int currentPlayerCoveredCount = ng.Board.Points.Count(p => p.Checkers.FirstOrDefault()?.Colour == currentPlayer);
                int otherPlayerCoveredCount = ng.Board.Points.Count(p => p.Checkers.FirstOrDefault()?.Colour == currentPlayer.GetNext());
                int moveAdvantage = (movedTurnPlay?.SumOfValuesPlayed ?? 0) - (nextMovedTurnPlay?.SumOfValuesPlayed ?? 0);
                int bearingOffPoints = movedTurnPlay?.PlayParts?.Count(tpp => tpp is BearedOffTurnPlayPart) ?? 0;

                return new
                {
                    tpgprntp.tp,
                    tpgprntp.g,
                    tpgprntp.p,
                    tpgprntp.r,
                    tpgprntp.ntp,
                    cpcc = currentPlayerCoveredCount,
                    opcc = otherPlayerCoveredCount,
                    ma = moveAdvantage,
                    bo = bearingOffPoints
                };
            })
            .GroupBy(tpgprntpcp => tpgprntpcp.tp)
            .SelectMany(gr => gr, (gr, tpgprntpcp) => new
            {
                minCc = gr.Min(tpgprntpcp => tpgprntpcp.cpcc - tpgprntpcp.opcc),
                minMa = gr.Min(tpgprntpcp => tpgprntpcp.ma),
                minBo = gr.Min(tpgprntpcp => tpgprntpcp.bo),
                tpgprntpcp
            })
            .OrderByDescending(tpgprntpcpmincp => tpgprntpcpmincp.minCc)
            .ThenByDescending(tpgprntpcpmincp => tpgprntpcpmincp.minMa)
            .ThenByDescending(tpgprntpcpmincp => tpgprntpcpmincp.minBo)
            .ToList();
        var bestTurnPlay = turnPlaysSorted.FirstOrDefault()?.tpgprntpcp.tp;

        return bestTurnPlay;
    }
}
// todo - player that prefers longer portes, check if places taken block oponent

public record StrategicPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        if (game.State != GameState.PlayerMovedOrTriedOrBearedOff
            && game.State != GameState.PlayerWonRollForOrder)
            return game.GetAllTurnPlays(roll).FirstOrDefault();
        PlayerColour currentPlayer = game.GetCurrentTurnPlayer().Value;
        PlayerColour oponentPlayer = currentPlayer.GetNext();
        List<TurnPlay> possibleTurnPlays = new List<TurnPlay>(game.GetAllTurnPlays(roll));

        var turnPlaysGames = possibleTurnPlays
            .Select(tp => new { tp, g = tp.ApplyToGame(game) });

        var winningTurnPlay = turnPlaysGames
            .FirstOrDefault(tpg => tpg.g.State.GameOver())?.tp;
        if (winningTurnPlay != null)
            return winningTurnPlay;

        var orderedTurnPlays = turnPlaysGames
            .Select(tpg =>
            {
                int longestBlockingPortesAdvantage = tpg.g.LongestBlockingPortes(currentPlayer);
                int blockingPointsAdvantage = tpg.g.BlockingPoints(currentPlayer);
                int takenPointsAdvantage = tpg.g.TakenPoints(currentPlayer);
                MovedTurnPlay movedTurnPlay = tpg.tp as MovedTurnPlay;
                int bearingOffAdvantage = movedTurnPlay?.PlayParts?.Count(tpp => tpp is BearedOffTurnPlayPart) ?? 0;

                return new
                {
                    tpg.tp,
                    tpg.g,
                    lbpa = longestBlockingPortesAdvantage,
                    bpa = blockingPointsAdvantage,
                    tpa = takenPointsAdvantage,
                    boa = bearingOffAdvantage
                };
            })
            .OrderByDescending(tpgp => tpgp.lbpa)
            .ThenByDescending(tpgp => tpgp.bpa)
            .ThenByDescending(tpgp => tpgp.boa)
            .ThenByDescending(tpgp => tpgp.tpa);

        return orderedTurnPlays.First().tp;
    }
}

public record AskStepsPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        List<TurnPlay> possibleTurnPlays = game.GetAllTurnPlays(roll, removeRedundantPlays: false).ToList();

        if (!possibleTurnPlays.Any())
            return null;

        var reminingTurnPlayElements = possibleTurnPlays.Select(tp => new { tp, pe = tp.GetPlayElements() });

        while (reminingTurnPlayElements.Count() > 1)
        {
            List<string> keys = reminingTurnPlayElements.GroupBy(tpe => tpe.pe.FirstOrDefault()).Select(gtpe => gtpe.Key.key).Where(k => k != null).Distinct().ToList();
            foreach (string possibleKey in keys)
                Console.WriteLine(possibleKey);
            Console.Write(">");
            string key;
            if (keys.Count == 1)
            {
                key = keys[0];
                Console.WriteLine(key);
            }
            else
                key = Console.ReadLine();

            List<string> values = reminingTurnPlayElements.GroupBy(tpe => tpe.pe.FirstOrDefault()).Select(gtpe => gtpe.Key.value).Where(v => v != null).Distinct().ToList();
            foreach (string possibleValue in values)
                Console.WriteLine(possibleValue);
            Console.Write(">");
            string value;
            if (values.Count == 1)
            {
                value = values[0];
                Console.WriteLine(value);
            }
            else
                value = Console.ReadLine();

            reminingTurnPlayElements = reminingTurnPlayElements
                .Where(rtpe => rtpe.pe.First().key == key && rtpe.pe.First().value == value)
                .Select(rtpe => new { rtpe.tp, pe = (IReadOnlyList<(string, string)>)rtpe.pe.Skip(1).ToList() });
        }

        return reminingTurnPlayElements.Single().tp;
    }
}
