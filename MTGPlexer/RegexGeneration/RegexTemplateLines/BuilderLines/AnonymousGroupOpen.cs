namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class AnonymousGroupOpen : EnclosureBookend, IGroupOpen
{
    public AnonymousGroupOpen(Enclosure[] enclosures)
        : base(enclosures, "(")
    {
    }

    public override string ToString() => base.ToString();
}