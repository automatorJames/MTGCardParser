namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record RegexCommentedLineSpan
(
    string SpanText,
    HexPalette Palette,
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