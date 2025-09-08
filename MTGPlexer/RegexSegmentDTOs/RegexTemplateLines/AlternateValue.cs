namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record AlternateValue(string Value, string Path, int Indentation, bool IsFirstAlternate = false) 
    : RegexTemplateLine(GetFormattedValue(Value, IsFirstAlternate), Path, Indentation)
{
    public Regex Regex { get; } = new Regex(Value, RegexOptions.Compiled);

    static string GetFormattedValue(string value, bool isFirstAlternate)
    {
        var firstChar = isFirstAlternate ? " " : "|";
        return firstChar + " " + value;
    }
}