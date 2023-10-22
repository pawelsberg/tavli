namespace Pawelsberg.Tavli.Model.Common;

public abstract record GameBase
{
    public abstract string StringRepresentation();
    public abstract bool GameOver();
    public abstract IEnumerable<PlayerColour> GetPlayersRolling();
    public abstract IEnumerable<TurnRoll> GetAllTurnRolls();
    public abstract PlayerColour? GetCurrentTurnPlayer();
    public abstract PlayerColour? GetStatePlayer();
    public abstract bool GetPlayerWonDouble();
}

public enum GameType
{
    Portes,
    Plakoto,
    Fevga,
    AssoDio
}
public static class GameTypeExtensions
{
    public static GameBase GetGameBeginning(this GameType thisGameType)
    {
        switch (thisGameType)
        {
            case GameType.Portes:
                return PlayingPortes.Game.GetBeginning();
            case GameType.Plakoto:
                return PlayingPlakoto.Game.GetBeginning();
            case GameType.Fevga:
                return PlayingFevga.Game.GetBeginning();
            case GameType.AssoDio:
                return PlayingAssoDio.Game.GetBeginning();
            default:
                throw new ArgumentException("Unknown game type");
        }
    }
    public static GameType GetGameType(this GameBase thisGameBase)
    {
        if (thisGameBase is PlayingPortes.Game)
            return GameType.Portes;
        else if (thisGameBase is PlayingPlakoto.Game)
            return GameType.Plakoto;
        else if (thisGameBase is PlayingFevga.Game)
            return GameType.Fevga;
        else if (thisGameBase is PlayingAssoDio.Game)
            return GameType.AssoDio;
        else throw new Exception("Unknown game type");
    }

    public static PlayerBase GetStrategicPlayer(this GameType thisGameType)
    {
        switch (thisGameType)
        {
            case GameType.Portes:
                return new PlayingPortes.StrategicPlayer();
            case GameType.Plakoto:
                return new PlayingPlakoto.StrategicPlayer();
            case GameType.Fevga:
                return new PlayingFevga.StrategicPlayer();
            case GameType.AssoDio:
                return new PlayingAssoDio.StrategicPlayer();
            default:
                throw new ArgumentException("Unknown game type");
        }
    }

    public static PlayerBase GetAskPlayer(this GameType thisGameType)
    {
        switch (thisGameType)
        {
            case GameType.Portes:
                return new PlayingPortes.AskPlayer();
            case GameType.Plakoto:
                return new PlayingPlakoto.AskPlayer();
            case GameType.Fevga:
                return new PlayingFevga.AskPlayer();
            case GameType.AssoDio:
                return new PlayingAssoDio.AskPlayer();
            default:
                throw new ArgumentException("Unknown game type");
        }
    }
}

