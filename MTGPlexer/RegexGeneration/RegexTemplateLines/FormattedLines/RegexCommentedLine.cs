namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

public record RegexCommentedLine
{
    public List<RegexCommentedLineSpan> Spans { get; }

    public string Regex { get; }
    public string Comment { get; }
    public string EnclosurePath { get; }
    public virtual string FullPath { get; }
    public int Ordinal { get; }
    public string FormattedText { get; }

    public RegexCommentedLine(string regex, string comment, string enclosurePath, int ordinal, List<RegexCommentedLineSpan> spans)
    {
        Regex = regex;
        Comment = comment;
        EnclosurePath = enclosurePath;
        FullPath = enclosurePath;
        Ordinal = ordinal;
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