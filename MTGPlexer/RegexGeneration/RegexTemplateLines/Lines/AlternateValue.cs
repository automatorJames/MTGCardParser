namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class AlternateValue : RegexElement, IMatchableAlternate
{
    public object CanonicalValue { get; }
    public int Ordinal { get; }
    public string CanonicalValueDisplay { get; }
    public Regex AlternateRegex { get; }

    public AlternateValue(Enclosure[] enclosures, string value, string comment, int ordinal)
        : base(
            enclosures,
            value,
            comment: comment
        )
    {
        CanonicalValue = value;
        CanonicalValueDisplay = value;
        AlternateRegex = new($"^{value}$", RegexOptions.Compiled);
        Ordinal = ordinal;
    }

    public override string ToString() => base.ToString();
}