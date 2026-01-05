namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class GroupOpen : EncloureBookend, IGroupOpen
{
    public GroupOpen(Enclosure[] enclosures)
        : base(enclosures, "(")
    {
    }

    public override string ToString() => base.ToString();
}