namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record AlternateValue(string Value, string Path, int Indentation, bool IsFirstAlternate = false) 
    : RegexTemplateLine(Value, Path, Indentation)
{
    public Regex Regex { get; } = new Regex(Value, RegexOptions.Compiled);
}