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
            GetFormattedValue(value, ordinal == 0),
            comment: comment
        )
    {
        CanonicalValue = value;
        CanonicalValueDisplay = value;
        AlternateRegex = new($"^{value}$", RegexOptions.Compiled);
    }

    static string GetFormattedValue(string value, bool isFirst)
    {
        var firstChar = isFirst ? "" : "|";
        return firstChar + value;
    }

    public override string ToString() => base.ToString();
}