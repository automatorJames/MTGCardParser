namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record BuiltRegex
(
    string MinifiedRegexString,
    Regex Regex,
    List<RegexFormattedLine> FormattedLines
);
