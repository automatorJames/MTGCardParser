namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record RegexTemplateLine
(
    Enclosure[] Enclosures, 
    string Regex, 
    Palette Palette = null,
    string Comment = null
)
{
    public string UniquePath { get; } = string.Join('.', Enclosures.Select(x => x.Ordinal));

    /// <summary>
    /// The dot-path name (including the root type) of the property path that this formatted regex line represents.
    /// </summary>
    public string NamedPath { get; } =
        Enclosures.Length == 0 ? string.Empty : // todo: Enclosures is sometimes empty b/c we're lazy when constructing boundaries; let's not do that
        Enclosures.OfType<RootEnclosure>().Single().RootTypeName
        + (Enclosures.Any(x => x is NamedEnclosure) ? "." + string.Join('.', Enclosures.OfType<NamedEnclosure>().Select(x => x.Name)) : "");

    public Enclosure[] PropEnclosures => Enclosures.OfType<NamedEnclosure>().ToArray();

    public override string ToString() =>
        Regex
        + (Comment == null ? "" : $" # {Comment}");   
}