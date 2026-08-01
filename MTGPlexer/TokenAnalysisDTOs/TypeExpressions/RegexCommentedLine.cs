namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record RegexCommentedLine
{
    public List<RegexCommentedLineSpan> Spans { get; set; }

    public string Regex { get; set; }
    public string Comment { get; }
    public string EnclosurePath { get; }
    public virtual string FullPath { get; }
    public string FormattedText { get; set; }

    public RegexCommentedLine(string regex, string comment, string enclosurePath, List<RegexCommentedLineSpan> spans)
    {
        Regex = regex;
        Comment = comment;
        EnclosurePath = enclosurePath;
        FullPath = enclosurePath;
        FormattedText = Regex + Comment;
        Spans = spans;
    }

    public static string GetRelativePath(string input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        int dotIndex = input.IndexOf('.');
        return dotIndex < 0 || dotIndex == input.Length - 1
            ? null
            : input[(dotIndex + 1)..];
    }
}