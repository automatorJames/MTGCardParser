namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record RegexCommentedLine(string Regex, string Comment, DeterministicPalette Palette)
{
    public string FormattedText { get; set; } = Regex + Comment;
}

