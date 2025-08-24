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

    private readonly Regex _regex = new(RegexMatchPattern ?? "", RegexOptions.Compiled);

    public bool CheckIfMatch(string stringToCheck)
    {
        if (Role < PrettifiedRegexLineRole.FirstEnumValueInGroup || RegexMatchPattern == null)
        {
            return false;
        }

        return _regex.IsMatch(stringToCheck);
    }

    public override string ToString() => DisplayText;
}

public enum PrettifiedRegexLineRole
{
    Empty,
    WordBoundary,
    CaptureGroupStart,
    CaptureGroupEnd,
    LiteralMatch,
    ConnectiveMatch,
    FirstEnumValueInGroup,
    NonFirstEnumValueInGroup,
    PatternValue
}