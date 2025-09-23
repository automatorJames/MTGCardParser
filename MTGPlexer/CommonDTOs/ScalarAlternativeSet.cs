namespace MTGPlexer.CommonDTOs;

public record ScalarAlternativeSet
(
    List<string> Alternatives
)
{
    public Regex CollectiveRegex { get; } = new(string.Join('|', Alternatives), RegexOptions.Compiled);
}

