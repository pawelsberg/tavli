namespace Pawelsberg.Tavli.Model.PlayingAssoDio;

public enum GameState
{
    Beginning,
    PlayersDrawRollForOrder,
    PlayerWonRollForOrder,
    PlayerMovedOrTriedOrBearedOffOrBoardedAfterTwoDifferent,
    PlayerMovedOrTriedOrBearedOffOrBoardedAfterAssoDioOrDouble,
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
