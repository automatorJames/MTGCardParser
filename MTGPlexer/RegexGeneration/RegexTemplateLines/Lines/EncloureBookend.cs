namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public abstract record EncloureBookend
(
    Enclosure[] Enclosures,
    string Regex,
    string Comment = null
)
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        Regex: Regex,
        Comment: Comment
    );