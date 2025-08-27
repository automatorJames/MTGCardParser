namespace MTGPlexer.TokenAnalysis.RegexDTOs;

using System.Text.RegularExpressions;

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

    private readonly Regex _regex = CreateRegex(RegexMatchPattern);

    private static Regex CreateRegex(string pattern)
    {
        try { return !string.IsNullOrEmpty(pattern) ? new Regex(pattern, RegexOptions.Compiled) : null; }
        catch { return null; }
    }

    public bool CheckIfMatch(string stringToCheck) => _regex?.IsMatch(stringToCheck) ?? false;
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
    EnumValue,
    Alternation,
    GroupAlternation,
    GenericGroupStart,
    GenericGroupEnd,
    TokenUnitOneOfHeader,
    Comment,
    CharacterRange
}