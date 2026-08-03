namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record SmartSpan
{
    public string Content { get; init; }
    public string FullyQualifiedName { get; }
    public HexPalette Palette { get; }

    public SmartSpan(string content, string fullyQualifiedName, HexPalette hexPalette)
    {
        Content = content;
        FullyQualifiedName = fullyQualifiedName;
        Palette = hexPalette;
    }

    public SmartSpan Mirror() =>
        this with { Content = string.Concat(Content.Reverse()) };

    public override string ToString() => Content;
}