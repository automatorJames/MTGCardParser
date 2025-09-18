namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record RegexCommentedLine(string Regex, string Comment, Palette Palette)
{
    public string FormattedText { get; set; } = Regex + Comment;
}

