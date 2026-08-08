namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Renders the comment-column span sequence for a group open/close brick: a corner glyph, a padding
/// line drawn in the group's border character, and the comment text itself — split into its constituent
/// role-tagged pieces (e.g. a group open's "Type" label and its ": UnderlyingType" disambiguator) so each
/// can be colored independently via <see cref="SmartSpanControlPanel"/>. Open bookends draw the box's top
/// edge with the comment first; close bookends draw the bottom edge with the comment last.
/// </summary>
internal class GroupBookendCommentRenderer
{
    readonly Func<RegexBrick, string, TokenRegexSpanKind?, SmartSpan> _spanFactory;
    readonly int _maxCommentLength;

    /// <param name="spanFactory">Builds a colored <see cref="SmartSpan"/> for a brick's text and (optional) span role, using the renderer's palette lookup.</param>
    /// <param name="maxCommentLength">The shared comment-column width every line's box must fit within, from <see cref="CommentBoxMetrics"/>.</param>
    public GroupBookendCommentRenderer(Func<RegexBrick, string, TokenRegexSpanKind?, SmartSpan> spanFactory, int maxCommentLength)
    {
        _spanFactory = spanFactory;
        _maxCommentLength = maxCommentLength;
    }

    /// <summary>Builds the corner, padding-line, and comment spans for one group open/close brick.</summary>
    public List<SmartSpan> Render(RegexBrickGroupBookend brick)
    {
        var boxChars = SmartRegexStaticRules.GetBoxCharsForBookendBrick(brick);
        var layout = GetLayout(brick, boxChars);
        var segments = GetCommentSegments(brick);

        var commentBuffer = string.Empty.PadLeft(SmartRegexStaticRules.GroupBookendCommentBuffer);
        var commentCoreLength = segments.Sum(x => x.Text.Length);
        var commentWithBufferLength = (commentBuffer.Length * 2) + commentCoreLength;
        var paddingLineLength = _maxCommentLength - CommentBoxMetrics.GetGroupBoxPaddingCount(brick.NestedDepth, isBookend: true) - commentWithBufferLength;
        var paddingLine = string.Empty.PadLeft(paddingLineLength, boxChars.Horizontal);

        var commentSpans = segments
            .Select((segment, i) =>
            {
                var prefix = i == 0 ? commentBuffer : "";
                var suffix = i == segments.Length - 1 ? commentBuffer : "";
                return _spanFactory(brick, prefix + segment.Text + suffix, segment.Kind);
            })
            .ToList();

        var paddingSpan = _spanFactory(brick, paddingLine, TokenRegexSpanKind.CommentGroupBorderWall);

        List<SmartSpan> spans = [_spanFactory(brick, layout.StartCorner.ToString(), TokenRegexSpanKind.CommentGroupBorderWall)];
        spans.AddRange(layout.CommentBeforePadding ? [.. commentSpans, paddingSpan] : [paddingSpan, .. commentSpans]);
        spans.Add(_spanFactory(brick, layout.EndCorner.ToString(), TokenRegexSpanKind.CommentGroupBorderWall));

        return spans;
    }

    /// <summary>The role-tagged text pieces making up a bookend's comment: an open brick's type label (plus disambiguator, if any), or a close brick's name (plus quantifier reminder, if any).</summary>
    static (string Text, TokenRegexSpanKind Kind)[] GetCommentSegments(RegexBrickGroupBookend brick) => brick switch
    {
        RegexBrickGroupOpen open => BuildSegments(
            (open.TypeLabel, TokenRegexSpanKind.CommentGroupOpenHeaderText),
            (open.TypeDisambiguator, TokenRegexSpanKind.CommentGroupOpenHeaderDisambiguator)),
        RegexBrickGroupClose close => BuildSegments(
            (close.NameText, TokenRegexSpanKind.CommentGroupFooterText),
            (close.QuantifierReminder, TokenRegexSpanKind.CommentGroupFooterQuantifierReminder)),
        _ => throw new ArgumentOutOfRangeException(nameof(brick), brick, $"Unhandled {nameof(RegexBrickGroupBookend)} subtype")
    };

    static (string, TokenRegexSpanKind)[] BuildSegments(params (string Text, TokenRegexSpanKind Kind)[] segments) =>
        segments.Where(x => !string.IsNullOrEmpty(x.Text)).ToArray();

    static (char StartCorner, char EndCorner, bool CommentBeforePadding) GetLayout(RegexBrickGroupBookend brick, BoxCharSet boxChars) => brick switch
    {
        RegexBrickGroupOpen => (boxChars.TopLeft, boxChars.TopRight, true),
        RegexBrickGroupClose => (boxChars.BottomLeft, boxChars.BottomRight, false),
        _ => throw new ArgumentOutOfRangeException(nameof(brick), brick, $"Unhandled {nameof(RegexBrickGroupBookend)} subtype")
    };
}
