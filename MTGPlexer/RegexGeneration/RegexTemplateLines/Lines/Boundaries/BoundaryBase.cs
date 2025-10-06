namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines.Boundaries;

public abstract class BoundaryBase : RegexElement
{
    protected BoundaryBase(Enclosure[] enclosures, string regex, string comment)
        : base(enclosures, regex, comment: comment)
    {
    }
}