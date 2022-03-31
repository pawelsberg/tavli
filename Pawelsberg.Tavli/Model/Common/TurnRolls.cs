namespace Pawelsberg.Tavli.Model.Common;

public record TurnRoll
{
    public IReadOnlyList<int> Values { get; set; }
    public decimal Probablity { get; set; }
    public TurnRoll() { }
    private TurnRoll(decimal probablity, params int[] values)
    {
        Values = new List<int>(values);
        Probablity = probablity;
    }
    public static IEnumerable<TurnRoll> NoRolling()
    {
        return new List<TurnRoll> { new TurnRoll(1) };
    }
    public static IEnumerable<TurnRoll> SinglePlayerRolls()
    {
        decimal singleProbablity = 1m / 36;
        return Enumerable
            .Range(1, 6)
            .SelectMany(r1 => Enumerable.Range(1, 6).Where(r2 => r2 <= r1).Select(r2 => new { r1, r2 }))
            .Select(r => new TurnRoll(r.r1 == r.r2 ? singleProbablity : singleProbablity * 2, r.r1, r.r2))
            .ToList();
    }
    public static IEnumerable<TurnRoll> TwoPlayersRoll()
    {
        decimal singleProbablity = 1m / 36;
        return Enumerable
            .Range(1, 6)
            .SelectMany(r1 => Enumerable.Range(1, 6).Select(r2 => new { r1, r2 }))
            .Select(r => new TurnRoll(singleProbablity, r.r1, r.r2))
            .ToList();
    }
    public string StringRepresentation()
    {
        return $"({string.Join(",", Values.Select(v => $"{v}"))})";
    }
}
