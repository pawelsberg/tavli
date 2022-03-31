using Newtonsoft.Json;
using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.Main;

public record Configuration
{
    public static string ConfigurationAppDataSubfolder = "Pawelsberg.Tavli";
    public static string ConfigurationFileName = "config.json";
    public bool AcceptedLicence { get; set; }
    public bool AccetedGamesSharing { get; set; }
    public Common.Version TavliVersion { get; set; }
    public bool Sound { get; set; }
    public double SoundVolume { get; set; }
    public List<GameConfiguration> GameConfigurations { get; set; }
    public static Configuration GetNew()
    {
        return new Configuration
        {
            AcceptedLicence = false,
            AccetedGamesSharing = false,
            TavliVersion = Common.Version.GetCurrent(),
            Sound = true,
            SoundVolume = 1.0,
            GameConfigurations = new List<GameConfiguration>
            {
                new GameConfiguration { GameType = GameType.Portes, WhiteBearOffOnLeft = false, WhiteBearOffOnTop = false },
                new GameConfiguration { GameType = GameType.Plakoto, WhiteBearOffOnLeft = true, WhiteBearOffOnTop = true},
                new GameConfiguration { GameType = GameType.Fevga, WhiteBearOffOnLeft = false, WhiteBearOffOnTop = false},
                new GameConfiguration { GameType = GameType.AssoDio, WhiteBearOffOnLeft = true, WhiteBearOffOnTop = true}
            }
        };
    }
    public static string ConfigurationFileFullPath
        => Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ConfigurationAppDataSubfolder,
                    ConfigurationFileName
                    );
    public static bool CanLoad()
    {
        return File.Exists(ConfigurationFileFullPath);
    }
    public static Configuration Load()
    {
        return JsonConvert.DeserializeObject<Configuration>(
            File.ReadAllText(ConfigurationFileFullPath));
    }
    public static void CreateAppDataSubfolderIfDoesntExist()
    {
        string directory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        ConfigurationAppDataSubfolder);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    public void Save()
    {
        File.WriteAllText(ConfigurationFileFullPath, JsonConvert.SerializeObject(this));
    }
}
