namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class TextLine : RegexElement
{
    public TextLine(Enclosure[] enclosures, string value)
        : base(enclosures, value.Replace(" ", "[ ]"), comment: "literal match")
    {
    }

    public override string ToString() => base.ToString();
}