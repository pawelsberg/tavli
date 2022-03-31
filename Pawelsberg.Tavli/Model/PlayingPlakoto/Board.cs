using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.PlayingPlakoto;

public record Board : Common.Board
{
    public Board(IReadOnlyList<Point> points) : base(points)
    {
    }

    public bool IsBearingPossible(PlayerColour playerColour)
    {
        int bearingStartPosition = playerColour == PlayerColour.White ? 0 : 18;
        int notBearingStartPosition = playerColour == PlayerColour.White ? 6 : 0;

        return
            !Points.Skip(notBearingStartPosition).Take(18).Any(p => p.ContainsPlayersCheckers(playerColour))
            && Points.Skip(bearingStartPosition).Take(6).Any(p => p.ContainsPlayersCheckers(playerColour));
    }

    public static Board BeginningBoard
    {
        get
        {
            List<Point> points = Enumerable.Range(0, 24).Select(pointIndex =>
            {
                if (pointIndex == 0)
                    return Point.CreateSingleColourPoint(15, PlayerColour.Black);
                else if (pointIndex == 23)
                    return Point.CreateSingleColourPoint(15, PlayerColour.White);
                else
                    return Point.Empty;

            }).ToList();
            return new Board(points);
        }
    }
}
