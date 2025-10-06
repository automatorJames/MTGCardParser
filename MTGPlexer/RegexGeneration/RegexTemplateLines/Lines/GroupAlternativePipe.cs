namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class GroupAlternativePipe : RegexElement
{
    public GroupAlternativePipe(Enclosure[] enclosures)
        : base(enclosures, "|", comment: "alternate divider")
    {
    }

    public override string ToString() => base.ToString();
}