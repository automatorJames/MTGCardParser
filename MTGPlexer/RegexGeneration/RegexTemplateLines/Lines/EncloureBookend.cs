namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public abstract class EncloureBookend : RegexElement
{
    protected EncloureBookend(Enclosure[] enclosures, string regex, string comment = null)
        : base(enclosures, regex, comment: comment)
    {
    }
}