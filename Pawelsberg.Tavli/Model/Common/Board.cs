namespace Pawelsberg.Tavli.Model.Common;

public abstract record Board
{
    // TODO fix this comment for all games
    /// <summary> 24 points of the board. 0 - whites home, 23 - blacks home </summary>
    public IReadOnlyList<Point> Points;

    public Board(IReadOnlyList<Point> points)
    {
        Points = points;
    }

    public bool IsSameAs(Board otherBoard)
    {
        if (Points.Count != otherBoard.Points.Count)
            return false;

        List<(Point pa, Point pb)> pointPairs = Points.Select((p, i) => (pa: p, pb: otherBoard.Points[i])).ToList();

        if (pointPairs.Any(pp => pp.pa is null != pp.pb is null))
            return false;

        if (pointPairs.Any(pp => pp.pa is not null && pp.pa.Checkers.Count != pp.pb.Checkers.Count))
            return false;

        if (pointPairs.Any(pp => pp.pa is not null && !pp.pa.Checkers.Select((c, i) => (c, i)).All(ci => ci.c.Colour == pp.pb.Checkers[ci.i].Colour)))
            return false;

        return true;
    }

    public int GenerateHashCode()
        => Points.Aggregate(0, (acc, p) => acc ^ p.GenerateHashCode());

    public Checker GetTopChecker(int position)
    {
        return Points[position].TopChecker;
    }

    public IReadOnlyList<Point> PointsWithReplacedPoints(IReadOnlyDictionary<int, Point> pointsOnPosition)
        => Points.Select((p, i) => pointsOnPosition.ContainsKey(i) ? pointsOnPosition[i] : p).ToList();

    public bool ContainsPlayersCheckers(PlayerColour playerColour)
    {
        return Points.Any(p => p.ContainsPlayersCheckers(playerColour));
    }

    public bool ContainsPlayersCheckers(PlayerColour playerColour, int startPosition, int endPosition)
    {
        return Points
            .Where((p, i) => i >= startPosition && i <= endPosition)
            .Any(p => p.ContainsPlayersCheckers(playerColour));
    }

    public static IReadOnlyList<Point> CreatePointsFromQuaters(List<List<PlayerColour?>> topRightBoard, List<List<PlayerColour?>> topLeftBoard, List<List<PlayerColour?>> bottomLeftBoard, List<List<PlayerColour?>> bottomRightBoard)
    {
        return Enumerable.Range(0, 24).Select(pos =>
        {
            if (pos < 6)
                return Point.CreateSingleColourPoint(topRightBoard.Select(s => s.ElementAt(6 - pos - 1)).Count(c => c.HasValue), topRightBoard[0][6 - pos - 1] ?? PlayerColour.White);
            else if (pos < 12)
                return Point.CreateSingleColourPoint(topLeftBoard.Select(s => s.ElementAt(12 - pos - 1)).Count(c => c.HasValue), topLeftBoard[0][12 - pos - 1] ?? PlayerColour.White);
            else if (pos < 18)
                return Point.CreateSingleColourPoint(bottomLeftBoard.Select(s => s.ElementAt(pos - 12)).Count(c => c.HasValue), bottomLeftBoard[bottomLeftBoard.Count - 1][pos - 12] ?? PlayerColour.White);
            else
                return Point.CreateSingleColourPoint(bottomRightBoard.Select(s => s.ElementAt(pos - 18)).Count(c => c.HasValue), bottomRightBoard[bottomRightBoard.Count - 1][pos - 18] ?? PlayerColour.White);
        }).ToList();

    }
}
