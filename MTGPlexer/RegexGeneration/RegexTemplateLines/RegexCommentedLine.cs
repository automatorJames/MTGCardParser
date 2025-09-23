namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record RegexCommentedLine
(
    string Regex, 
    string Comment,
    string EnclosurePath,
    Dictionary<int, string> ColorSpans,
    Regex MatchRegex
)
{
    public string FormattedText { get; } = Regex + Comment;
}