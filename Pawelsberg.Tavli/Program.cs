using System.Text;
using Pawelsberg.Tavli.Model.Common;
using Pawelsberg.Tavli.Model.Extensions;
using Pawelsberg.Tavli.Model.Main;
using Version = Pawelsberg.Tavli.Model.Common.Version;

namespace Pawelsberg.Tavli;

public class Program
{
    static Random _random = new Random();
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.Unicode;
        Configuration configuration = Configuration.CanLoad()
            ? Configuration.Load()
            : Configuration.GetNew();
        configuration.Save();

        Console.WriteLine($"TAVLI {configuration.TavliVersion.ReleaseMajor}.{configuration.TavliVersion.ReleaseMinor} Copyright © Pawel Welsberg 2023");

        Console.WriteLine();
        Console.WriteLine($"Choose one of ({String.Join(", ", Enum.GetNames(typeof(GameType)))})");
        Console.Write("GameType>");
        string gameTypeText = Console.ReadLine();
        GameType gameType = Enum.Parse<GameType>(gameTypeText);
        GameBase gameBeginning = gameType.GetGameBeginning();
        
        Console.WriteLine();
        Console.WriteLine($"Choose black player type ({String.Join(", ", Enum.GetNames(typeof(PlayerType)))})");
        Console.Write("BlackPlayerType>");
        string blackPlayerTypeText = Console.ReadLine();
        PlayerType blackPlayerType = Enum.Parse<PlayerType>(blackPlayerTypeText);
        PlayerBase blackPlayer = blackPlayerType switch
        {
            PlayerType.Computer => gameType.GetStrategicPlayer(),
            PlayerType.Human => gameType.GetAskPlayer()
        };
        
        Console.WriteLine();
        Console.WriteLine($"Choose white player type ({String.Join(", ", Enum.GetNames(typeof(PlayerType)))})");
        Console.Write("WhitePlayerType>");
        string whitePlayerTypeText = Console.ReadLine();
        PlayerType whitePlayerType = Enum.Parse<PlayerType>(whitePlayerTypeText);
        PlayerBase whitePlayer = whitePlayerType switch
        {
            PlayerType.Computer => gameType.GetStrategicPlayer(),
            PlayerType.Human => gameType.GetAskPlayer()
        };
        
        Console.WriteLine();
        Console.WriteLine(gameBeginning.StringRepresentation());


        DateTime startDateTime = DateTime.UtcNow;
        (GameBase endGame, List<Turn> allTurns) = (game: gameBeginning, turns: new List<Turn>())
            .For(gt => !gt.game.GameOver(), gt =>
          {
              GameBase g = gt.game;
              Console.WriteLine();
              Console.WriteLine($"Rolling players: {string.Join(", ", g.GetPlayersRolling()) }");
              List<TurnRoll> rolls = g.GetAllTurnRolls().ToList();
              TurnRoll roll = PickTurnRoll(rolls);
              Console.WriteLine($"Rolled:{roll.StringRepresentation()}");
              PlayerColour? currentTurnPlayer = g.GetCurrentTurnPlayer();
              TurnPlayBase turnPlay = currentTurnPlayer == PlayerColour.Black 
                  ? blackPlayer.ChooseTurnPlay(g, roll) : 
                  whitePlayer.ChooseTurnPlay(g, roll);

              Console.WriteLine($"Playing:");
              Console.WriteLine(turnPlay.StringRepresentation());
              GameBase ng = turnPlay.ApplyToGame(g);
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
            PlayerWon = endGame.GetStatePlayer()!.Value,
            GameWonDouble = endGame.GetPlayerWonDouble(),
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
