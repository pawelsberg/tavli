namespace Pawelsberg.Tavli.Model.Common;

public record Turn
{
    public TurnRoll Roll { get; set; }
    public TurnPlayBase Play { get; set; }
}
