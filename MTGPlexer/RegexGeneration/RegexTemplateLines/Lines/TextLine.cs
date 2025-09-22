namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record TextLine
(
    Enclosure[] Enclosures,
    string Value
) 
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        Regex: Value.Replace(" ", "[ ]"),
        Comment: "literal match"
    )
{
    public override string ToString() => base.ToString();
}
