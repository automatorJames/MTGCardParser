namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record SpaceLine
(
    Enclosure[] Enclosures
) 
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        Regex: "[ ]",
        Comment: "connective space"
    );