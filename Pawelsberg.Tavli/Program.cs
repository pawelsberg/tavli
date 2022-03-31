using Pawelsberg.Tavli.Model.Common;
using Pawelsberg.Tavli.Model.Extensions;
using Pawelsberg.Tavli.Model.Main;
using Pawelsberg.Tavli.Model.PlayingPortes;
using Version = Pawelsberg.Tavli.Model.Common.Version;

namespace Pawelsberg.Tavli;

public class Program
{
    static Random _random = new Random();
    static void Main(string[] args)
    {

        Configuration configuration = Configuration.CanLoad()
            ? Configuration.Load()
            : Configuration.GetNew();
        configuration.Save();

        Game game = Game.GetBeginning();
        //Game game = Game.ParseFromStringRepresentation(File.ReadAllText("game.txt"));
        Console.WriteLine(game.StringRepresentation());
        //bool whitePlayerBlocked = game.Board.IsPlayerBlocked(PlayerColour.Black);
        //MinPipsInTwoRoundsBetterEndingPlayer blackPlayer = new MinPipsInTwoRoundsBetterEndingPlayer();
        //MinPipsInTwoRoundsBetterEndingPlayer whitePlayer = new MinPipsInTwoRoundsBetterEndingPlayer();
        //BestCoverageInTwoRoundsPlayer blackPlayer = new BestCoverageInTwoRoundsPlayer();
        //BestCoverageInTwoRoundsPlayer whitePlayer = new BestCoverageInTwoRoundsPlayer();
        //AskPlayer blackPlayer = new AskPlayer();
        //AskPlayer whitePlayer = new AskPlayer();
        StrategicPlayer blackPlayer = new StrategicPlayer();
        StrategicPlayer whitePlayer = new StrategicPlayer();
        //BestCoverageInTwoRoundsPlayer whitePlayer = new BestCoverageInTwoRoundsPlayer();
        //AskStepsPlayer whitePlayer = new AskStepsPlayer();
        //AskStepsPlayer blackPlayer = new AskStepsPlayer();
        //Console.WriteLine(game.StringRepresentation());

        DateTime startDateTime = DateTime.UtcNow;
        (Game endGame, List<Turn> allTurns) = (game, turns: new List<Turn>())
            .For(gt => !gt.game.State.GameOver(), gt =>
          {
              Game g = gt.game;
              Console.WriteLine();
              Console.WriteLine($"Rolling players: {string.Join(", ", g.GetPlayersRolling()) }");
              List<TurnRoll> rolls = g.GetAllTurnRolls().ToList();
              TurnRoll roll = PickTurnRoll(rolls);
              Console.WriteLine($"Rolled:{roll.StringRepresentation()}");
              PlayerColour? currentTurnPlayer = g.GetCurrentTurnPlayer();
              TurnPlay turnPlay;
              if (currentTurnPlayer == PlayerColour.Black)
                  turnPlay = blackPlayer.ChooseTurnPlay(g, roll);
              else
                  turnPlay = whitePlayer.ChooseTurnPlay(g, roll);

              Console.WriteLine($"Playing:");
              Console.WriteLine(turnPlay.StringRepresentation());
              Game ng = turnPlay.ApplyToGame(g);
              Console.WriteLine(ng.StringRepresentation());
              return (game: ng, gt.turns.Concat(new List<Turn> { new Turn { Roll = roll, Play = turnPlay } }).ToList());
          });
        DateTime endDateTime = DateTime.UtcNow;
        new GameTranscript
        {
            TavliVersion = Version.GetCurrent(),
            GameType = endGame.GetGameType(),
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            WhitePlayerType = PlayerType.Computer,
            BlackPlayerType = PlayerType.Computer,
            PlayerWon = endGame.StatePlayer.Value,
            GameWonDouble = endGame.State == GameState.PlayerWonDouble,
            Turns = allTurns
        }.Save();
        Console.ReadLine();
    }

    public static TurnRoll PickTurnRoll(List<TurnRoll> rolls)
    {
        if (!rolls.Any())
            return null;

        Console.Write("Roll>");
        string rollText = Console.ReadLine();

        if (int.TryParse(rollText, out int rollInt))
        {
            int dice1 = rollInt / 10;
            int dice2 = rollInt % 10;
            if (dice1 < 1 || dice1 > 6 || dice2 < 1 || dice2 > 6)
                throw new Exception("Wrong roll");

            TurnRoll exactRoll = rolls.FirstOrDefault(r => r.Values.Count == 2 && r.Values[0] == dice1 && r.Values[1] == dice2);
            if (exactRoll != null)
                return exactRoll;

            TurnRoll flippedRoll = rolls.FirstOrDefault(r => r.Values.Count == 2 && r.Values[0] == dice2 && r.Values[1] == dice1);
            if (flippedRoll != null)
                return flippedRoll;
            else
                throw new Exception("Unexpected list of rolls");

        }
        else if (rollText.Equals("r", StringComparison.OrdinalIgnoreCase))
            return rolls[_random.Next(0, rolls.Count - 1)];
        else
            throw new Exception("Wrong roll");
    }
}
