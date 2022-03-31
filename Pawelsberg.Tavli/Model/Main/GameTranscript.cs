using Newtonsoft.Json;
using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.Main;

public record GameTranscript
{
    public static string GameTranscriptsFileName = "transcripts.json";
    public Common.Version TavliVersion { get; set; }
    public GameType GameType { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public PlayerType WhitePlayerType { get; set; }
    public PlayerType BlackPlayerType { get; set; }
    public PlayerColour PlayerWon { get; set; }
    public bool GameWonDouble { get; set; }
    public List<Turn> Turns { get; set; }
    public static string GameTranscriptsFileFullPath
                    => Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Configuration.ConfigurationAppDataSubfolder,
                    GameTranscriptsFileName
                    );
    public static bool CanLoad()
    {
        return File.Exists(GameTranscriptsFileFullPath);
    }
    public void Save()
    {
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        IReadOnlyList<GameTranscript> oldGameTranscripts = CanLoad()
            ? JsonConvert.DeserializeObject<List<GameTranscript>>(
            File.ReadAllText(GameTranscriptsFileFullPath)
            , jsonSerializerSettings
            )
            : new List<GameTranscript>();

        IReadOnlyList<GameTranscript> gameTranscripts = oldGameTranscripts.Concat(new GameTranscript[] { this }).ToList();
        Configuration.CreateAppDataSubfolderIfDoesntExist();
        File.WriteAllText(GameTranscriptsFileFullPath, JsonConvert.SerializeObject(gameTranscripts, jsonSerializerSettings));
    }
}
