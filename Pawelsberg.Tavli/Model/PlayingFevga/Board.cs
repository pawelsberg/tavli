using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.PlayingFevga;

public record Board : Common.Board
{
    public Board(IReadOnlyList<Point> points) : base(points)
    {
    }

    public bool IsBearingPossible(PlayerColour playerColour)
    {
        int bearingStartPosition = playerColour == PlayerColour.White ? 18 : 6;
        int bearingEndPosition = playerColour == PlayerColour.White ? 23 : 11;

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

    public bool AreAllCheckersOnTheFirstHalf(PlayerColour playerColour)
    {
        int firstHalfStartPosition = playerColour == PlayerColour.White ? 0 : 12;
        int halfLength = 12;
        int checkersInFirstHalf = Points
            .Skip(firstHalfStartPosition)
            .Take(halfLength)
            .Sum(p => p.Checkers.Count(c => c.Colour == playerColour));
        return checkersInFirstHalf == 15;
    }

    public static Board BeginningBoard
    {
        get
        {
            List<Point> points = Enumerable.Range(0, 24).Select(pointIndex =>
            {
                if (pointIndex == 0)
                    return Point.CreateSingleColourPoint(15, PlayerColour.White);
                else if (pointIndex == 12)
                    return Point.CreateSingleColourPoint(15, PlayerColour.Black);
                else
                    return Point.Empty;

            }).ToList();
            return new Board(points);
        }
    }

    public bool AllHomePositionsTaken(PlayerColour playerColour)
    {
        int homeStartPosition = playerColour == PlayerColour.White ? 0 : 12;
        int homeEndPosition = playerColour == PlayerColour.White ? 5 : 17;
        return Points
            .Select((p, i) => (p, i))
            .Where(pi => pi.i >= homeStartPosition && pi.i <= homeEndPosition)
            .All(pi => pi.p.Checkers.Any() && pi.p.Checkers[0].Colour == playerColour);

    }

    public bool IsPlayerBlocked(PlayerColour playerColour)
    {
        PlayerColour oponentPlayerColour = playerColour.GetNext();
        if (!ContainsPlayersCheckers(playerColour) || IsBearingPossible(playerColour))
            return false;
        return Enumerable
             .Range(0, 24)
             .Select(i => new { i, p = Points[i] })
             .Where(ip => ip.p.Checkers.FirstOrDefault()?.Colour == playerColour)
             .All(ip => Enumerable
                 .Range(ip.i + 1, 6)
                 .Where(oi => !(oi >= (playerColour == PlayerColour.White ? 24 : 12)
                 && ip.i < (playerColour == PlayerColour.White ? 24 : 12))) // not crossed the bear off area
                 .Select(oi => oi % 24)
                 .Select(oi => new { oi, op = Points[oi] })
                 .All(oip => oip.op.Checkers.FirstOrDefault()?.Colour == oponentPlayerColour));
    }
}
