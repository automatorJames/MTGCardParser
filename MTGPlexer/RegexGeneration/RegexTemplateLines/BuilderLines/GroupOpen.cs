namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class GroupOpen : EncloureBookend
{
    public GroupOpen(Enclosure[] enclosures)
        : base(enclosures, "(")
    {
    }

    public override string ToString() => base.ToString();
}