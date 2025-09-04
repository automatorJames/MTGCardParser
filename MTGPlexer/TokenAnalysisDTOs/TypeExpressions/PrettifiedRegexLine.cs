namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record PrettifiedRegexLine
(
    int LineNumber,
    string PropertyCaptureGroup,
    string Text,
    string RegexMatchPattern,
    PrettifiedRegexLineRole Role
)
{
    public string DisplayText { get; init; } = "";
    public int IndentLevel { get; init; } = 0;
    public string Comment { get; init; }
    public DeterministicPalette Palette { get; set; }

    private readonly Regex _regex = CreateRegex(RegexMatchPattern);

    private static Regex CreateRegex(string pattern)
    {
        try { return !string.IsNullOrEmpty(pattern) ? new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase) : null; }
        catch { return null; }
    }

    public bool CheckIfMatch(string stringToCheck, string propertyCaptureGroup)
    {
        var shouldSkip =
            PropertyCaptureGroup == null
            || propertyCaptureGroup != PropertyCaptureGroup
            || Role < PrettifiedRegexLineRole.EnumValue
            || RegexMatchPattern == "[ ]?"; // This pattern is automatically added to bool TokenUnitTypes

        if (shouldSkip)
            return false;

        return _regex?.IsMatch(stringToCheck) ?? false;
    }

    public override string ToString() => DisplayText;
}

public enum PrettifiedRegexLineRole
{
    Error,
    Separator,
    WordBoundary,
    CaptureGroupStart,
    CaptureGroupEnd,
    ConnectiveMatch,
    Alternation,
    GroupAlternation,
    GenericGroupStart,
    GenericGroupEnd,
    TokenUnitOneOfHeader,
    Comment,
    EnumValue,
    CharacterRange
}