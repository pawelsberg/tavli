using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.PlayingPortes;

public record Board : Common.Board
{
    public Board(IReadOnlyList<Point> points) : base(points)
    {
    }

    public bool IsBearingPossible(PlayerColour playerColour)
    {
        int bearingStartPosition = playerColour == PlayerColour.White ? 18 : 0;
        int notBearingStartPosition = playerColour == PlayerColour.White ? 0 : 6;

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
                    return Point.CreateSingleColourPoint(2, PlayerColour.White);
                else if (pointIndex == 5)
                    return Point.CreateSingleColourPoint(5, PlayerColour.Black);
                else if (pointIndex == 7)
                    return Point.CreateSingleColourPoint(3, PlayerColour.Black);
                else if (pointIndex == 11)
                    return Point.CreateSingleColourPoint(5, PlayerColour.White);
                else if (pointIndex == 12)
                    return Point.CreateSingleColourPoint(5, PlayerColour.Black);
                else if (pointIndex == 16)
                    return Point.CreateSingleColourPoint(3, PlayerColour.White);
                else if (pointIndex == 18)
                    return Point.CreateSingleColourPoint(5, PlayerColour.White);
                else if (pointIndex == 23)
                    return Point.CreateSingleColourPoint(2, PlayerColour.Black);
                else
                    return Point.Empty;

            }).ToList();
            return new Board(points);
        }
    }

}
