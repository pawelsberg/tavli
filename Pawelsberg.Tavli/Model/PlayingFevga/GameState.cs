namespace Pawelsberg.Tavli.Model.PlayingFevga;

public enum GameState
{
    Beginning,
    PlayersDrawRollForOrder,
    PlayerWonRollForOrder,
    PlayerMovedOrTriedOrBearedOff,
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
