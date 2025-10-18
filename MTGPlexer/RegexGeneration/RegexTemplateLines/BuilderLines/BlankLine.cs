namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class BlankLine : RegexElement
{
    public BlankLine(Enclosure[] enclosures)
        : base(enclosures, "")
    {
    }
}