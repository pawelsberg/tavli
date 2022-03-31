namespace Pawelsberg.Tavli.Model.EqualityComparers;

public class ListOfIntsEqualityComparer : IEqualityComparer<List<int>>
{
    public ListOfIntsEqualityComparer() { }
    public bool Equals(List<int> x, List<int> y)
    {
        if (x is null || y is null || x.Count != y.Count)
            return false;

        foreach (int i in Enumerable.Range(0, x.Count))
            if (x[i] != y[i])
                return false;
        return true;
    }

    public int GetHashCode(List<int> list)
    {
        return list.Aggregate(0, (acc, value) => 31 * acc + value.GetHashCode());
    }
}
