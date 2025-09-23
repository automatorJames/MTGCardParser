namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record RegexCommentedLine(string Regex, string Comment, Dictionary<int, string> ColorSpans)
{
    public string FormattedText { get; set; } = Regex + Comment;
}