namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public abstract class EnclosureBookend : RegexElement
{
    protected EnclosureBookend(Enclosure[] enclosures, string regex, string comment = null)
        : base(enclosures, regex, comment: comment)
    {
    }
}