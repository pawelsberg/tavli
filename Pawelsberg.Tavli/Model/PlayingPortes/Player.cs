using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.PlayingPortes;

public abstract record Player
{
    public abstract TurnPlay ChooseTurnPlay(Game game, TurnRoll roll);
}

public record QuickPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        return game.GetAllTurnPlays(roll).FirstOrDefault();
    }
}

public record MinPipsPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        if (game.State != GameState.PlayerMovedOrTriedOrBearedOffOrBoarded && game.State != GameState.PlayerWonRollForOrder)
            return game.GetAllTurnPlays(roll).FirstOrDefault();
        PlayerColour currentPlayer = game.GetCurrentTurnPlayer().Value;

        List<TurnPlay> possibleTurnPlays = new List<TurnPlay>(game.GetAllTurnPlays(roll));

        var turnPlaysPips = possibleTurnPlays
            .Select(tp => new { tp, g = tp.ApplyToGame(game) })
            .Select(tpg => new { tpg.tp, pips = tpg.g.GetPips(currentPlayer) - tpg.g.GetPips(currentPlayer.GetNext()) });
        int minPips = turnPlaysPips.Min(tpp => tpp.pips);

        return turnPlaysPips.Where(tpp => tpp.pips == minPips).FirstOrDefault()?.tp;
    }
}

public record MinPipsInTwoRoundsPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        if (game.State != GameState.PlayerMovedOrTriedOrBearedOffOrBoarded && game.State != GameState.PlayerWonRollForOrder)
            return game.GetAllTurnPlays(roll).FirstOrDefault();
        PlayerColour currentPlayer = game.GetCurrentTurnPlayer().Value;

        List<TurnPlay> possibleTurnPlays = new List<TurnPlay>(game.GetAllTurnPlays(roll));

        var turnPlaysGames = possibleTurnPlays
            .Select(tp => new { tp, g = tp.ApplyToGame(game) });

        var winningTurnPlay = turnPlaysGames
            .FirstOrDefault(tpg => tpg.g.State.GameOver())?.tp;
        if (winningTurnPlay != null)
            return winningTurnPlay;

        var turnPlaysMaxNpips = turnPlaysGames
            .Select(tpg => new { tpg.tp, tpg.g, p = tpg.g.GetPlayersRolling().Single() })
            .SelectMany(tpgp => tpgp.g.GetAllTurnRolls(), (tpgp, r) => new { tpgp.tp, tpgp.g, tpgp.p, r })
            .SelectMany(tpgpr => tpgpr.g.GetAllTurnPlays(tpgpr.r), (tpgpr, ntp) => new { tpgpr.tp, tpgpr.g, tpgpr.p, tpgpr.r, ntp })
            .Select(tpgprntp => new { tpgprntp.tp, tpgprntp.g, tpgprntp.p, tpgprntp.r, tpgprntp.ntp, ng = tpgprntp.ntp.ApplyToGame(tpgprntp.g) })
            .Select(tpgprntpng => new { tpgprntpng.tp, tpgprntpng.g, tpgprntpng.p, tpgprntpng.r, tpgprntpng.ntp, tpgprntpng.ng, npips = tpgprntpng.ng.GetPips(currentPlayer) - tpgprntpng.ng.GetPips(currentPlayer.GetNext()) })
            .GroupBy(tpgprntpngnpips => tpgprntpngnpips.tp)
            .Select(gr => new { tp = gr.Key, maxNpips = gr.Max(tpgprntpngnpips => tpgprntpngnpips.npips) });

        int minPips = turnPlaysMaxNpips.Min(tpmaxnpips => tpmaxnpips.maxNpips);

        return turnPlaysMaxNpips.Where(tpmaxnpips => tpmaxnpips.maxNpips == minPips).FirstOrDefault()?.tp;
    }
}

public record MinPipsInTwoRoundsBetterEndingPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        if (game.State != GameState.PlayerMovedOrTriedOrBearedOffOrBoarded && game.State != GameState.PlayerWonRollForOrder)
            return game.GetAllTurnPlays(roll).FirstOrDefault();
        PlayerColour currentPlayer = game.GetCurrentTurnPlayer().Value;

        List<TurnPlay> possibleTurnPlays = new List<TurnPlay>(game.GetAllTurnPlays(roll));

        var turnPlaysGames = possibleTurnPlays
            .Select(tp => new { tp, g = tp.ApplyToGame(game) });

        var winningTurnPlay = turnPlaysGames
            .FirstOrDefault(tpg => tpg.g.State.GameOver())?.tp;
        if (winningTurnPlay != null)
            return winningTurnPlay;

        var turnPlaysMaxNpips = turnPlaysGames
            .Select(tpg => new { tpg.tp, tpg.g, p = tpg.g.GetPlayersRolling().Single() })
            .SelectMany(tpgp => tpgp.g.GetAllTurnRolls(), (tpgp, r) => new { tpgp.tp, tpgp.g, tpgp.p, r })
            .SelectMany(tpgpr => tpgpr.g.GetAllTurnPlays(tpgpr.r), (tpgpr, ntp) => new { tpgpr.tp, tpgpr.g, tpgpr.p, tpgpr.r, ntp })
            .Select(tpgprntp => new { tpgprntp.tp, tpgprntp.g, tpgprntp.p, tpgprntp.r, tpgprntp.ntp, ng = tpgprntp.ntp.ApplyToGame(tpgprntp.g) })
            .Select(tpgprntpng => new { tpgprntpng.tp, tpgprntpng.g, tpgprntpng.p, tpgprntpng.r, tpgprntpng.ntp, tpgprntpng.ng, npips = tpgprntpng.ng.GetPips(currentPlayer) - tpgprntpng.ng.GetPips(currentPlayer.GetNext()) })
            .GroupBy(tpgprntpngnpips => tpgprntpngnpips.tp)
            .Select(gr => new { tp = gr.Key, maxNpips = gr.Max(tpgprntpngnpips => tpgprntpngnpips.npips) }).ToList();

        int minPips = turnPlaysMaxNpips.Min(tpmaxnpips => tpmaxnpips.maxNpips - ((MovedTurnPlay)tpmaxnpips.tp).PlayParts.Count(tpp => tpp is BearedOffTurnPlayPart));

        return turnPlaysMaxNpips.Where(tpmaxnpips => tpmaxnpips.maxNpips - ((MovedTurnPlay)tpmaxnpips.tp).PlayParts.Count(tpp => tpp is BearedOffTurnPlayPart) == minPips).FirstOrDefault()?.tp;
    }
}

public record StrategicPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        if (game.State != GameState.PlayerMovedOrTriedOrBearedOffOrBoarded
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
                int singleEndangeredCheckersDisadvantage = tpg.g.CheckersProneToBeating(currentPlayer);
                int currentPlayerPipsDisadvantage = tpg.g.GetPips(currentPlayer);
                int oponentPipsAdvantage = tpg.g.GetPips(oponentPlayer);
                int pipsAdvantage = oponentPipsAdvantage - currentPlayerPipsDisadvantage;
                int longestBlockingPortesAdvantage = tpg.g.LongestBlockingPortes(currentPlayer);
                MovedTurnPlay movedTurnPlay = tpg.tp as MovedTurnPlay;
                int bearingOffAdvantage = movedTurnPlay?.PlayParts?.Count(tpp => tpp is BearedOffTurnPlayPart) ?? 0;

                return new
                {
                    tpg.tp,
                    tpg.g,
                    secsd = singleEndangeredCheckersDisadvantage,
                    psa = pipsAdvantage,
                    lbpsa = longestBlockingPortesAdvantage,
                    boa = bearingOffAdvantage
                };
            })
            .OrderBy(tpgp => tpgp.secsd)
            .ThenByDescending(tpgp => tpgp.psa)
            .ThenByDescending(tpgp => tpgp.lbpsa)
            .ThenByDescending(tpgp => tpgp.boa);

        return orderedTurnPlays.First().tp;
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
