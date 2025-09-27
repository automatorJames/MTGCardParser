namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

public record RegexCommentedLineSpan
(
    string SpanText,
    Palette Palette,
    string PathRelativeToRoot,
    SpanHighlightTreatment HighlightTreatment,
    SpanLowlightTreatment LowlightTreatment
);

public enum SpanHighlightTreatment
{
    None,
    TextToHexLight,
    TextToHexSat,
    BackgroundToHex
}

public enum SpanLowlightTreatment
{
    None,
    TextToHexDark
}


