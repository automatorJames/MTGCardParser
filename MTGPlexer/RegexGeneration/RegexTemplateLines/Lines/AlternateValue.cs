namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class AlternateValue : RegexElement
{
    public object CanonicalValue { get; }

    public AlternateValue(Enclosure[] enclosures, string value, string comment)
        : base(
            enclosures,
            value,
            comment: comment
        )
    {
        CanonicalValue = value;
    }

    public override string ToString() => base.ToString();
}