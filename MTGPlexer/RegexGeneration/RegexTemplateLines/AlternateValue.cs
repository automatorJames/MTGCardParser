namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record AlternateValue
(
    string Value, 
    string Path, 
    int Indentation, 
    Palette Palette,
    RegexPropInfo Group,
    bool IsFirst, 
    bool IsOnly
) 
    : RegexTemplateLine(GetFormattedValue(Value, IsFirst), Path, Indentation, Palette, GetComment(IsOnly), Group: Group)
{
    public Regex Regex { get; } = new Regex(Value, RegexOptions.Compiled);

    static string GetFormattedValue(string value, bool isFirstAlternate)
    {
        var formattedValue = value.Replace(" ", "[ ]");
        var firstChar = isFirstAlternate ? " " : "|";
        formattedValue = firstChar + " " + formattedValue;
        return formattedValue;
    }

    static string GetComment(bool isOnlyAlternate) => isOnlyAlternate ? "match" : "alternate";

}