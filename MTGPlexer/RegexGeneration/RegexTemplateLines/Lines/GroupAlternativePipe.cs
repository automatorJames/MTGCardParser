namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record GroupAlternativePipe
(
    Enclosure[] Enclosures
) 
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        Regex: $"|",
        Comment: "alternate divider"
    )
{
    public override string ToString() => base.ToString();
}