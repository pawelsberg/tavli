namespace Pawelsberg.Tavli.Model.PlayingPlakoto;

public enum GameState
{
    Beginning,
    PlayersDrawRollForOrder,
    PlayerWonRollForOrder,
    PlayerMovedOrTiredOrBearedOff,
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
