namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class AlternateValue : RegexElement, IMatchableAlternate
{
    public object CanonicalValue { get; }
    public string CanonicalValueDisplay { get; }
    public Regex AlternateRegex { get; }

    public AlternateValue(Enclosure[] enclosures, string value, string comment, bool isFirst)
        : base(
            enclosures,
            GetFormattedValue(value, isFirst),
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