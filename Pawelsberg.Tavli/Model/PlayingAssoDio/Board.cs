using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.PlayingAssoDio;

public record Board : Common.Board
{
    public Board(IReadOnlyList<Point> points) : base(points)
    {
    }

    public bool IsBearingPossible(PlayerColour playerColour)
    {
        int bearingStartPosition = playerColour == PlayerColour.White ? 0 : 18;
        int bearingEndPosition = playerColour == PlayerColour.White ? 5 : 23;

        bool checkersBeforeBearingRange = Points
            .Take(bearingStartPosition)
            .Any(p => p.ContainsPlayersCheckers(playerColour));
        bool checkersInBearingRange = Points
            .Skip(bearingStartPosition)
            .Take(bearingEndPosition - bearingStartPosition + 1)
            .Any(p => p.ContainsPlayersCheckers(playerColour));
        bool checkersAfterBearingRange = Points
            .Skip(bearingEndPosition + 1)
            .Take(23 - bearingEndPosition)
            .Any(p => p.ContainsPlayersCheckers(playerColour));

        return !checkersBeforeBearingRange && checkersInBearingRange && !checkersAfterBearingRange;
    }

    public static Board BeginningBoard
    {
        get
        {
            List<Point> points = Enumerable.Range(0, 24).Select(pointIndex =>
            {
                return Point.Empty;
            }).ToList();
            return new Board(points);
        }
    }

    public (int startSourcePosition, int endSourcePosition) GetSourceZone(PlayerColour playerColour)
    {
        if (playerColour == PlayerColour.White)
        {
            int? lastCheckerLocation = Points
                .Select((p, i) => new { p, i })
                .Reverse()
                .FirstOrDefault(pi => pi.p.Checkers?.FirstOrDefault()?.Colour == playerColour)?.i;
            if (lastCheckerLocation == null)
                throw new Exception("Error getting source zone: no checkers on the board");
            if (lastCheckerLocation < 6)
                return (0, 5);
            else if (lastCheckerLocation < 12)
                return (6, 11);
            else if (lastCheckerLocation < 18)
                return (12, 17);
            else
                return (18, 23);
        }
        else
        {
            int? firstCheckerLocation = Points
                .Select((p, i) => new { p, i })
                .FirstOrDefault(pi => pi.p.Checkers?.FirstOrDefault()?.Colour == playerColour)?.i;
            if (firstCheckerLocation == null)
                throw new Exception("Error getting source zone: no checkers on the board");
            if (firstCheckerLocation >= 18)
                return (18, 23);
            else if (firstCheckerLocation >= 12)
                return (12, 17);
            else if (firstCheckerLocation >= 6)
                return (6, 11);
            else
                return (0, 5);
        }
    }

    internal bool CheckersInTheFirstQuarter(PlayerColour playerColour)
    {
        int firstQuarterStart = playerColour == PlayerColour.White ? 18 : 0;
        return Points.Skip(firstQuarterStart).Take(6).Any(p => p.Checkers.FirstOrDefault()?.Colour == playerColour);
    }

}
