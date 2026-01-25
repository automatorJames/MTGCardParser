namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class AtomElement : RegexElement
{
    public AtomElement(Enclosure[] enclosures, string value, string comment)
        : base(
            enclosures,
            value.Replace(" ", "[ ]"),
            comment: comment)
    {
    }

    public override string ToString() => base.ToString();
}