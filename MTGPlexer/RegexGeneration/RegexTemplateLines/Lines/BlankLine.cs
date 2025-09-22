namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record BlankLine
(
    Enclosure[] Enclosures
) 
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        Regex: ""
    );