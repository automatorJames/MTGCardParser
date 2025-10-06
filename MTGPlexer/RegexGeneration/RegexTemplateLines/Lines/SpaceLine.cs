namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class SpaceLine : RegexElement
{
    public SpaceLine(Enclosure[] enclosures)
        : base(enclosures, "[ ]", comment: "connective space")
    {
    }
}