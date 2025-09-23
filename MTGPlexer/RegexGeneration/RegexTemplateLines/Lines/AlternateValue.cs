namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record AlternateValue
(
    Enclosure[] Enclosures,
    string Value, 
    int Ordinal, 
    int TotalOptions
) 
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        GetFormattedValue(Value, Ordinal), 
        Comment: GetComment(Ordinal, TotalOptions)
    )
{
    public Regex MatchRegex { get; } = new Regex(Value, RegexOptions.Compiled);

    static string GetFormattedValue(string value, int ordinal)
    {
        var formattedValue = value.Replace(" ", "[ ]");
        var firstChar = ordinal == 0 ? " " : "|";
        formattedValue = firstChar + " " + formattedValue;
        return formattedValue;
    }

    static string GetComment(int ordinal, int totalOptions) => $"{ordinal + 1}/{totalOptions}";

    public override string ToString() => base.ToString();
}