namespace Pawelsberg.Tavli.Model.Common;

public enum PlayerColour
{
    White,
    Black
}

public static class PlayerColourExtensions
{
    public static PlayerColour GetNext(this PlayerColour playerColour)
    {
        return playerColour == PlayerColour.White
            ? PlayerColour.Black
            : PlayerColour.White;
    }
    public static char ToCharacter(this PlayerColour playerColour)
    {
        switch (playerColour)
        {
            case PlayerColour.White:
                return '☻';
            case PlayerColour.Black:
                return '☺';
            default:
                throw new ArgumentException("Unknown palyer colour");
        }
    }
    public static PlayerColour? ToPlayerColourNullable(char character)
    {
        switch (character)
        {
            case '☻':
                return PlayerColour.White;
            case '☺':
                return PlayerColour.Black;
            case ' ':
                return null;
            default:
                throw new Exception($"Cannot parse player colour:{character}");
        }
    }
    public static IEnumerable<PlayerColour> AllPlayerColours()
    {
        yield return PlayerColour.White;
        yield return PlayerColour.Black;
    }
}
