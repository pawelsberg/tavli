namespace Pawelsberg.Tavli.Model.Extensions;

public static class PermuteExtensions
{
    public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> sequence)
    {
        if (sequence is null)
            yield break;

        List<T> list = sequence.ToList();

        if (!list.Any())
            yield return Enumerable.Empty<T>();
        else
            foreach ((T startingElement, int index) in list.Select((se, i) => (se, i)))
            {
                IEnumerable<T> remainingItems = list.Where((e, i) => i != index);
                foreach (IEnumerable<T> permutationOfRemainder in remainingItems.Permute())
                    yield return startingElement.Concat(permutationOfRemainder);
            }
    }

    private static IEnumerable<T> Concat<T>(this T firstElement, IEnumerable<T> secondSequence)
    {
        yield return firstElement;
        if (secondSequence is null)
            yield break;

        foreach (var item in secondSequence)
            yield return item;
    }
}
