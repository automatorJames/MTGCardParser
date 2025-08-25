namespace MTGPlexer.TokenAnalysis.RegexDTOs;

using System.Text.RegularExpressions;

public record PrettifiedRegexLine
(
    int LineNumber,
    string CaptureGroupName,
    string Text,
    string RegexMatchPattern,
    PrettifiedRegexLineRole Role
)
{
    public string DisplayText { get; init; } = "";
    public int IndentLevel { get; init; } = 0; // Crucial for hierarchical formatting

    // Regex compilation is now handled more safely.
    private readonly Regex _regex = CreateRegex(RegexMatchPattern);

    private static Regex CreateRegex(string pattern)
    {
        try { return !string.IsNullOrEmpty(pattern) ? new Regex(pattern, RegexOptions.Compiled) : null; }
        catch { return null; } // Failsafe against invalid patterns
    }

    public bool CheckIfMatch(string stringToCheck) => _regex?.IsMatch(stringToCheck) ?? false;
    public override string ToString() => DisplayText;
}

public enum PrettifiedRegexLineRole
{
    Error, // For displaying parsing errors
    Empty,
    WordBoundary,
    CaptureGroupStart,
    CaptureGroupEnd,
    LiteralMatch,
    ConnectiveMatch,
    FirstEnumValueInGroup,
    NonFirstEnumValueInGroup,
    Alternation,
    PatternValue,
    GenericGroupStart,
    GenericGroupEnd,
    TokenUnitOneOfHeader,
    Quantifier,
    CharacterClass
}