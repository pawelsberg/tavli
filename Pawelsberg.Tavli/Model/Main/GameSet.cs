using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.Main;

public record GameSet
{
    public int WinningPoints { get; set; }
    public PlayerColour HumanPlayerColour { get; set; }
    public IReadOnlyList<GameType> GameTypes { get; set; }
    public int WhitePlayerPoints { get; set; }
    public int BlackPlayerPoints { get; set; }
}
