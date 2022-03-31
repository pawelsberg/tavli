using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.PlayingAssoDio;

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

public record StrategicPlayer : Player
{
    public override TurnPlay ChooseTurnPlay(Game game, TurnRoll roll)
    {
        if (game.State != GameState.PlayerMovedOrTriedOrBearedOffOrBoardedAfterAssoDioOrDouble
            && game.State != GameState.PlayerMovedOrTriedOrBearedOffOrBoardedAfterTwoDifferent
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
                int singleEndangeredCheckersDisadvantage = tpg.g.SingleEndangeredCheckers(currentPlayer);
                int portesAdvantage = tpg.g.Portes(currentPlayer);
                int currentPlayerPips = tpg.g.GetPips(currentPlayer);
                int oponentPips = tpg.g.GetPips(oponentPlayer);
                int pipsAdvantage = oponentPips - currentPlayerPips;
                MovedTurnPlay movedTurnPlay = tpg.tp as MovedTurnPlay;
                int bearingOffAdvantage = movedTurnPlay?.PlayParts?.Count(tpp => tpp is BearedOffTurnPlayPart) ?? 0;
                int hardBeatingAdvantage = tpg.g.Board.CheckersInTheFirstQuarter(oponentPlayer) ? 0 : movedTurnPlay.Beatings();

                return new
                {
                    tpg.tp,
                    tpg.g,
                    hba = hardBeatingAdvantage,
                    secsd = singleEndangeredCheckersDisadvantage,
                    poa = portesAdvantage,
                    pia = pipsAdvantage,
                    boa = bearingOffAdvantage
                };
            })
            .OrderByDescending(tpgp => tpgp.hba)
            .ThenBy(tpgp => tpgp.secsd)
            .ThenByDescending(tpgp => tpgp.pia)
            .ThenByDescending(tpgp => tpgp.poa)
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
