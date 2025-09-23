namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record RegexTemplateLine
(
    Enclosure[] Enclosures, 
    string Regex, 
    Palette Palette = null,
    string Comment = null
)
{
    public string Path { get; } = string.Join('_', Enclosures.Select(x => x.Ordinal));
    public string NamedPath { get; } = string.Join('_', Enclosures.OfType<NamedEnclosure>().Select(x => x.Name));
    public int CommentLength { get; } = Comment?.Length ?? 0;

    public override string ToString() =>
        Regex
        + (Comment == null ? "" : $" # {Comment}");

    
}