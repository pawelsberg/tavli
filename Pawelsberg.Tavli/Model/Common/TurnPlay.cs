namespace Pawelsberg.Tavli.Model.Common;

public abstract record TurnPlayBase
{
    public abstract GameBase ApplyToGame(GameBase g);
    public abstract string StringRepresentation();
    public abstract IReadOnlyList<(string key, string value)> GetPlayElements();
}
