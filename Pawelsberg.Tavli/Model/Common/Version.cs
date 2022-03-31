namespace Pawelsberg.Tavli.Model.Common;

public record Version
{
    public DateTime TimeStamp { get; set; }
    public int ReleaseMajor { get; set; }
    public int ReleaseMinor { get; set; }
    public string StringRepresentation() => $"v{ReleaseMajor}.{ReleaseMinor} ({TimeStamp})";
    public static Version GetCurrent()
        => new Version { ReleaseMajor = 1, ReleaseMinor = 1, TimeStamp = new DateTime(2021, 7, 25) };
}
