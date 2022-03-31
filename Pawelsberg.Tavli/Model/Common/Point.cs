namespace Pawelsberg.Tavli.Model.Common;

public record Point
{
    /// <summary>  Point's checkers. 0 - bottom.  </summary>
    public IReadOnlyList<Checker> Checkers;

    private Point(IReadOnlyList<Checker> checkers)
    {
        Checkers = checkers;
    }
    private Point()
    {
        Checkers = new List<Checker>();
    }

    public static Point CreateSingleColourPoint(int count, PlayerColour colour)
    {
        List<Checker> checkers = Enumerable.Range(0, count).Select(i => new Checker(colour)).ToList();
        return new Point(checkers);
    }
    public static Point Empty => new Point();

    public Point WithAddedOnTheTopChecker(PlayerColour colour) =>
        new Point(Checkers.Concat(new List<Checker> { new Checker(colour) }).ToList());
    public Point WithRemovedCheckerFromTheTop() =>
        new Point(Checkers.Take(Checkers.Count - 1).ToList());
    public Checker TopChecker => Checkers.LastOrDefault();
    public bool ContainsPlayersCheckers(PlayerColour playerColour)
        => Checkers.Any(c => c.Colour == playerColour);

    public bool IsSameAs(Point otherPoint)
    {
        if (Checkers.Count != otherPoint.Checkers.Count)
            return false;
        if (Checkers.Select((c, i) => otherPoint.Checkers[i] == c).Any(same => !same))
            return false;

        return true;
    }

    public int GenerateHashCode()
    {
        return Checkers.Aggregate(0, (acc, c) => acc * 2 ^ c.GetHashCode());
    }
}
