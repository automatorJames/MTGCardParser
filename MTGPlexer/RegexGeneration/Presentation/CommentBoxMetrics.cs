namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Column-width measurements shared across an entire formatted regex: where the '#' comment separator
/// starts, and how wide the comment column needs to be to fit every brick's comment plus the nested
/// group-box walls surrounding it.
/// </summary>
internal class CommentBoxMetrics
{
    /// <summary>The column (from line start) where every line's '#' comment separator begins.</summary>
    public int CommentSeparatorColumn { get; }

    /// <summary>The widest comment column needed by any brick, including its surrounding group-box wall padding.</summary>
    public int MaxCommentLength { get; }

    public CommentBoxMetrics(List<RegexBrick> bricks)
    {
        CommentSeparatorColumn = bricks
            .Max(x => x.RegexFormatted.Length + SmartRegexStaticRules.GetIndentSpaces(x))
            + SmartRegexStaticRules.CommentBorderLineBuffer;

        MaxCommentLength = bricks.Max(GetCommentLengthIncludingGroupBoxPadding);
    }

    static int GetCommentLengthIncludingGroupBoxPadding(RegexBrick brick)
    {
        int length = brick.CommentFormatted.Length + GetGroupBoxPaddingCount(brick.NestedDepth, brick is RegexBrickGroupBookend);

        if (brick is RegexBrickGroupBookend)
            length += SmartRegexStaticRules.GroupBookendCommentBuffer * 2;

        return length;
    }

    /// <summary>
    /// Chars consumed by group-box wall structure around a brick's comment: one wall (plus inner buffer)
    /// per level of nesting on each side, plus room for a bookend's corner glyph. <paramref name="nestedDepth"/>
    /// should be the actual live nesting depth at the brick's rendered position — not necessarily the
    /// brick's own <see cref="RegexBrick.NestedDepth"/>, which synthetic bricks (e.g. blank separator lines)
    /// don't always track accurately.
    /// </summary>
    public static int GetGroupBoxPaddingCount(int nestedDepth, bool isBookend) =>
        (nestedDepth * 2)
        + (nestedDepth * SmartRegexStaticRules.GroupWallInnerBuffer * 2)
        + (isBookend ? 2 : 0);
}
