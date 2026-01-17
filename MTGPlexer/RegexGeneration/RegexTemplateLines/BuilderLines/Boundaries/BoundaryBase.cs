namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Boundaries;

public abstract class BoundaryBase : RegexElement
{
    protected BoundaryBase(Enclosure[] enclosures, string regex, string comment, bool doNotAddPrecedingSpace = false)
        : base(enclosures, regex, comment: comment, doNotAddPrecedingSpace: doNotAddPrecedingSpace)
    {
    }
}