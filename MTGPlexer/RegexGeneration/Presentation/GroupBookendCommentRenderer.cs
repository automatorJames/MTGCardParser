namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Renders the comment-column span sequence for a group open/close brick: a corner glyph, a padding
/// line drawn in the group's border character, and the comment text itself. Open bookends draw the
/// box's top edge with the comment first; close bookends draw the bottom edge with the comment last.
/// </summary>
internal class GroupBookendCommentRenderer
{
    readonly Func<RegexBrick, string, SmartSpan> _spanFactory;
    readonly int _maxCommentLength;

    /// <param name="spanFactory">Builds a colored <see cref="SmartSpan"/> for a brick's text, using the renderer's palette lookup.</param>
    /// <param name="maxCommentLength">The shared comment-column width every line's box must fit within, from <see cref="CommentBoxMetrics"/>.</param>
    public GroupBookendCommentRenderer(Func<RegexBrick, string, SmartSpan> spanFactory, int maxCommentLength)
    {
        _spanFactory = spanFactory;
        _maxCommentLength = maxCommentLength;
    }

    /// <summary>Builds the corner, padding-line, and comment spans for one group open/close brick.</summary>
    public List<SmartSpan> Render(RegexBrickGroupBookend brick)
    {
        var boxChars = SmartRegexStaticRules.GetBoxCharsForBookendBrick(brick);
        var layout = GetLayout(brick, boxChars);

        var commentBuffer = string.Empty.PadLeft(SmartRegexStaticRules.GroupBookendCommentBuffer);
        var commentWithBuffer = $"{commentBuffer}{brick.CommentFormatted}{commentBuffer}";
        var paddingLineLength = _maxCommentLength - CommentBoxMetrics.GetGroupBoxPaddingCount(brick) - commentWithBuffer.Length;
        var paddingLine = string.Empty.PadLeft(paddingLineLength, boxChars.Horizontal);

        var commentSpan = _spanFactory(brick, commentWithBuffer);
        var paddingSpan = _spanFactory(brick, paddingLine);

        List<SmartSpan> spans = [_spanFactory(brick, layout.StartCorner.ToString())];
        spans.AddRange(layout.CommentBeforePadding ? [commentSpan, paddingSpan] : [paddingSpan, commentSpan]);
        spans.Add(_spanFactory(brick, layout.EndCorner.ToString()));

        return spans;
    }

    static (char StartCorner, char EndCorner, bool CommentBeforePadding) GetLayout(RegexBrickGroupBookend brick, BoxCharSet boxChars) => brick switch
    {
        RegexBrickGroupOpen => (boxChars.TopLeft, boxChars.TopRight, true),
        RegexBrickGroupClose => (boxChars.BottomLeft, boxChars.BottomRight, false),
        _ => throw new ArgumentOutOfRangeException(nameof(brick), brick, $"Unhandled {nameof(RegexBrickGroupBookend)} subtype")
    };
}
