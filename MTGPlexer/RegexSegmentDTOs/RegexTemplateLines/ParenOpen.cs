namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record GroupOpen(string Path, int Indentation) : RegexTemplateLine("(", Path, Indentation);

public enum GroupEnclosure
{
    Parentheses,
    SquareBrackets,
    CurlyBrackets
}
