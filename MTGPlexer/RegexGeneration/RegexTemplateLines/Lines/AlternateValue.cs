namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class AlternateValue : RegexElement, IMatchableAlternate
{
    public object CanonicalValue { get; }
    public string CanonicalValueDisplay { get; }
    public Regex AlternateRegex { get; }

    public AlternateValue(Enclosure[] enclosures, string value, string comment)
        : base(
            enclosures,
            value,
            comment: comment
        )
    {
        CanonicalValue = value;
        CanonicalValueDisplay = value;
        AlternateRegex = new($"^{value}$", RegexOptions.Compiled);
    }

    public override string ToString() => base.ToString();
}