namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record AlternateValue
(
    Enclosure[] Enclosures,
    string Value, 
    Palette Palette,
    bool IsFirst, 
    bool IsOnly
) 
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        GetFormattedValue(Value, IsFirst), 
        Palette: Palette,
        Comment: GetComment(IsOnly)
    )
{
    public Regex MatchRegex { get; } = new Regex(Value, RegexOptions.Compiled);

    static string GetFormattedValue(string value, bool isFirstAlternate)
    {
        var formattedValue = value.Replace(" ", "[ ]");
        var firstChar = isFirstAlternate ? " " : "|";
        formattedValue = firstChar + " " + formattedValue;
        return formattedValue;
    }

    static string GetComment(bool isOnlyAlternate) => isOnlyAlternate ? "match" : "alternate";

    public override string ToString() => base.ToString();
}