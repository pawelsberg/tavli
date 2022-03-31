namespace Pawelsberg.Tavli.Model.PlayingPortes;

public enum GameState
{
    Beginning,
    PlayersDrawRollForOrder,
    PlayerWonRollForOrder,
    PlayerMovedOrTriedOrBearedOffOrBoarded,
    PlayerWonSingle,
    PlayerWonDouble
}

public static class GameStateExtension
{
    public static bool GameOver(this GameState thisGameState)
    {
        return thisGameState == GameState.PlayerWonSingle || thisGameState == GameState.PlayerWonDouble;
    }
}
