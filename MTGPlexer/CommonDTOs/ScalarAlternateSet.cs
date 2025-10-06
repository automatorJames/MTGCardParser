namespace MTGPlexer.CommonDTOs;

public record ScalarAlternateSet
(
    List<string> Alternates
)
{
    public Regex CollectiveRegex { get; } = new(string.Join('|', Alternates), RegexOptions.Compiled);
}

