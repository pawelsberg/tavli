namespace Pawelsberg.Tavli.Model.Common;

public abstract record GameBase
{
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
    public static GameBase CreateNew(this GameType thisGameType)
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
}

