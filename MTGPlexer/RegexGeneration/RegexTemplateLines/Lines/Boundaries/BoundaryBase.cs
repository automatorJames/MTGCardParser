namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines.Boundaries;

public abstract record BoundaryBase
(
    Enclosure[] Enclosures,
    string Regex,
    string Comment
)
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        Regex: Regex,
        Comment: Comment
    );