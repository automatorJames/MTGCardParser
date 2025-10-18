namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public abstract class EncloureBookend : RegexElement
{
    protected EncloureBookend(Enclosure[] enclosures, string regex, string comment = null)
        : base(enclosures, regex, comment: comment)
    {
    }
}