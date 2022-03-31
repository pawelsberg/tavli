namespace Pawelsberg.Tavli.Model.Common;

public record Checker
{
    public PlayerColour Colour;
    public Checker(PlayerColour colour)
    {
        Colour = colour;
    }
}
