namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class SmartLineFactory
{
    List<RegexBrick> _bricks;
    Dictionary<NamedGroupNode, HexPalette> _namedGroupPalettes;
    int _commentBorderLineIndex;
    int _maxFormattedCommentLength;
    Stack<SmartSpan> _currentEnclosingLeftSideWallSpanStack = [];
    List<SmartSpan> _currentEnclosingRightSideWallMirroredSpans = [];

    public SmartLineFactory(List<RegexBrick> bricks, RegexGraph regexGraph)
    {
        _bricks = bricks;

        _commentBorderLineIndex = bricks
            .Max(x =>
                x.RegexFormatted.Length
                + (x.NestedDepth * SmartRegexStaticRules.IndentSpaces))
                + SmartRegexStaticRules.CommentBorderLineBuffer;

        _namedGroupPalettes = DeterministicPalette
            .GetPositionalPaletteSet(regexGraph.NamedGroupFlatGraph.Values, positionalOverrideColors: HexColor.WhiteSmoke);

        _maxFormattedCommentLength = GetMaxCommentLengthIncludingGroupBoxes();
    }

    public static List<SmartLine> Get(List<RegexBrick> bricks, RegexGraph regexGraph)
    {
        SmartLineFactory factory = new(bricks, regexGraph);
        return factory.GetLines();
    }

    List<SmartLine> GetLines()
    {
        List<SmartLine> lines = [];

        foreach (var brick in _bricks)
        {
            List<SmartSpan> spans = [];

            if (brick is RegexBrickGroupClose)
                PopGroupFromSpanStack();

            AddRegexSpan(spans, brick);
            AddBorderLine(spans);
            AddCommentSpans(spans, brick);

            lines.Add(new(spans));

            if (brick is RegexBrickGroupOpen groupOpenBrick)
                PushGroupOpenToSpanStack(groupOpenBrick);
        }

        return lines;
    }

    int GetMaxCommentLengthIncludingGroupBoxes()
    {
        int maxBrickCommentLength = 0;

        foreach (var brick in _bricks)
        {
            int brickCommentLength =
                brick.Comment.Length
                + GetGroupBoxPaddingCount(brick);

            if (brick is RegexBrickGroupBookend)
                brickCommentLength += SmartRegexStaticRules.GroupBookendCommentBuffer * 2;

            maxBrickCommentLength = Math.Max(maxBrickCommentLength, brickCommentLength);
        }

        return maxBrickCommentLength;
    }

    /// <summary>
    /// Calculates the number of chars consumed by group box structure around
    /// a RegexBrick comment, including enclosing walls, inner wall buffers,
    /// and bookend corners where applicable.
    /// </summary>
    int GetGroupBoxPaddingCount(RegexBrick brick) =>
        (brick.NestedDepth * 2)
        + (brick.NestedDepth * SmartRegexStaticRules.GroupWallInnerBuffer * 2)
        + (brick is RegexBrickGroupBookend ? 2 : 0);

    void PushGroupOpenToSpanStack(RegexBrickGroupOpen groupOpenBrick)
    {
        var boxCharSet = SmartRegexStaticRules.GetBoxCharsForBookendBrick(groupOpenBrick);
        var paddedWall = $"{boxCharSet.Vertical}{string.Empty.PadLeft(SmartRegexStaticRules.GroupWallInnerBuffer)}";
        var span = SpanFromBrick(groupOpenBrick, paddedWall);

        _currentEnclosingLeftSideWallSpanStack.Push(span);

        _currentEnclosingRightSideWallMirroredSpans = _currentEnclosingLeftSideWallSpanStack
            .Select(x => x.Mirror())
            .ToList();
    }

    void PopGroupFromSpanStack()
    {
        _currentEnclosingLeftSideWallSpanStack.Pop();

        _currentEnclosingRightSideWallMirroredSpans = _currentEnclosingLeftSideWallSpanStack
            .Select(x => x.Mirror())
            .ToList();
    }

    SmartSpan SpanFromBrick(RegexBrick brick, string formattedContent) =>
        new(formattedContent, brick.FullyQualifiedName, _namedGroupPalettes[brick.NamedGroupParent]);

    void AddRegexSpan(List<SmartSpan> spans, RegexBrick brick)
    {
        var content = string.Empty.PadLeft(brick.NestedDepth * SmartRegexStaticRules.IndentSpaces) + brick.RegexFormatted;
        content = content.PadRight(_commentBorderLineIndex);
        spans.Add(SpanFromBrick(brick, content));
    }

    void AddBorderLine(List<SmartSpan> spans)
    {
        spans.Add(new(
            SmartRegexStaticRules.CommentBorderLineWithBuffer,
            "",
            DeterministicPalette.GetStaticPalette(HexColor.DarkSlateGrey)));
    }

    void AddCommentSpans(List<SmartSpan> spans, RegexBrick brick)
    {
        spans.AddRange(_currentEnclosingLeftSideWallSpanStack.Reverse());

        if (brick is RegexBrickGroupBookend groupBookendBrick)
            AddCommentContentSpansForGroupBookend(spans, groupBookendBrick);
        else
            AddRegularCommentContentSpan(spans, brick);

        spans.AddRange(_currentEnclosingRightSideWallMirroredSpans);
    }

    void AddCommentContentSpansForGroupBookend(List<SmartSpan> spans, RegexBrickGroupBookend groupBookendBrick)
    {
        var boxCharSet = SmartRegexStaticRules.GetBoxCharsForBookendBrick(groupBookendBrick);
        char paddingLineChar = boxCharSet.Horizontal;
        char startCornerChar;
        char endCornerChar;
        bool commentAppearsBeforePaddingLine;

        switch (groupBookendBrick)
        {
            case RegexBrickGroupOpen:
                startCornerChar = boxCharSet.TopLeft;
                endCornerChar = boxCharSet.TopRight;
                commentAppearsBeforePaddingLine = true;
                break;

            case RegexBrickGroupClose:
                startCornerChar = boxCharSet.BottomLeft;
                endCornerChar = boxCharSet.BottomRight;
                commentAppearsBeforePaddingLine = false;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(groupBookendBrick));
        }

        spans.Add(SpanFromBrick(groupBookendBrick, startCornerChar.ToString()));

        var commentBuffer = string.Empty.PadLeft(SmartRegexStaticRules.GroupBookendCommentBuffer);
        var commentContent = $"{commentBuffer}{groupBookendBrick.Comment}{commentBuffer}";

        var paddingLineLength =
            _maxFormattedCommentLength
            - GetGroupBoxPaddingCount(groupBookendBrick)
            - commentContent.Length;

        var paddingLine = string.Empty.PadLeft(paddingLineLength, paddingLineChar);

        SmartSpan commentContentSpan = SpanFromBrick(groupBookendBrick, commentContent);
        SmartSpan paddingLineSpan = SpanFromBrick(groupBookendBrick, paddingLine);

        spans.AddRange(commentAppearsBeforePaddingLine
            ? [commentContentSpan, paddingLineSpan]
            : [paddingLineSpan, commentContentSpan]);

        spans.Add(SpanFromBrick(groupBookendBrick, endCornerChar.ToString()));
    }

    void AddRegularCommentContentSpan(List<SmartSpan> spans, RegexBrick brick)
    {
        var commentContent = brick.Comment.PadRight(
            _maxFormattedCommentLength - GetGroupBoxPaddingCount(brick));

        SmartSpan commentContentSpan = SpanFromBrick(brick, commentContent);
        spans.Add(commentContentSpan);
    }
}