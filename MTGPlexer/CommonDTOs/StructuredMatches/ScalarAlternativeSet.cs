namespace MTGPlexer.CommonDTOs.StructuredMatches;

public record ScalarAlternativeSet
(
    List<string> Alternatives
)
{
    public Regex Regex { get; } = new(string.Join('|', Alternatives), RegexOptions.Compiled);
}

