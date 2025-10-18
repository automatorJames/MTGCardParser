using MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Alternates;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

/// <summary>
/// A centralized record to hold all the treatment rules for the generated regex,
/// such as highlighting and lowlighting.
/// </summary>
internal record FormattedRegexTreatmentRules
{
    /// <summary>
    /// Defines the lowlight treatment for all spans in the comment section (right of the '#').
    /// </summary>
    public SpanLowlightTreatment CommentLowlightTreatment { get; } = SpanLowlightTreatment.TextToHexDark;

    /// <summary>
    /// Determines the highlight treatment for the regex portion of a line (left of the '#').
    /// </summary>
    public SpanHighlightTreatment GetRegexHighlightTreatment(RegexElement line)
    {
        // Explicit Rule: AlternateValues on the regex side are ALWAYS TextToHexLight.
        if (line is AlternateValue)
        {
            return SpanHighlightTreatment.TextToHexLight;
        }

        // For all other line types, the regex side's text highlight mimics the comment side's text highlight.
        return GetCommentHighlightTreatment(line, isTextSpan: true);
    }

    /// <summary>
    /// Determines the highlight treatment for a span within the comment section.
    /// </summary>
    /// <param name="line">The context of the entire line being processed.</param>
    /// <param name="isTextSpan">True if the span is for comment text content; false for borders or fillers.</param>
    public SpanHighlightTreatment GetCommentHighlightTreatment(RegexElement line, bool isTextSpan)
    {
        if (isTextSpan)
        {
            return line switch
            {
                NamedGroupOpen => SpanHighlightTreatment.TextToHexSat,
                NamedGroupClose => SpanHighlightTreatment.TextToHexSat,
                AlternateValue => SpanHighlightTreatment.BackgroundToHex,
                _ => SpanHighlightTreatment.TextToHexLight,
            };
        }

        // Default for non-text spans (borders, fillers, etc.)
        return SpanHighlightTreatment.TextToHexLight;
    }
}