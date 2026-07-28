namespace MTGPlexer;

public class GlobalSettings
{
    public string SqlConnString { get; init; }
    public int? MaxSetSequence { get; init; }
    public bool IncludeEmptyDocuments { get; init; }
}
