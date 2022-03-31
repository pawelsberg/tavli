namespace Pawelsberg.Tavli.Model.PlayingPortes;

public class MovedTurnPlayEqualityComparer : IEqualityComparer<MovedTurnPlay>
{
    public MovedTurnPlayEqualityComparer() { }
    public bool Equals(MovedTurnPlay x, MovedTurnPlay y)
    {
        if (x.PlayParts.Count != y.PlayParts.Count)
            return false;

#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
        if (x.PlayParts.Select((xpp, i) => y.PlayParts[i] == xpp).Any(same => !same))
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            return false;

        return true;
    }

    public int GetHashCode(MovedTurnPlay turnPlay)
    {
        return turnPlay.PlayParts.Aggregate(0, (acc, pp) => acc ^ pp.GetHashCode());
    }
}

