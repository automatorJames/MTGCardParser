namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Renders an already-formatted brick sequence (see <see cref="RegexBrickFormattingPipeline"/>) into
/// <see cref="SmartLine"/>s: one line per brick, pairing its padded regex text with a colored, box-drawn
/// comment. Delegates group-box corner/padding-line rendering to <see cref="GroupBookendCommentRenderer"/>
/// and nested-wall bookkeeping to <see cref="GroupWallSpanTracker"/>; this class owns per-line assembly
/// and named-group palette lookup. Replaces what used to be the monolithic <c>SmartLineFactory</c>.
/// </summary>
public class SmartLineRenderer
{
    readonly List<RegexBrick> _bricks;
    readonly Dictionary<NamedGroupNode, HexPalette> _namedGroupPalettes;
    readonly CommentBoxMetrics _metrics;
    readonly GroupWallSpanTracker _wallTracker = new();
    readonly GroupBookendCommentRenderer _bookendRenderer;

    SmartLineRenderer(List<RegexBrick> bricks, RegexGraph regexGraph)
    {
        _bricks = bricks;
        _metrics = new CommentBoxMetrics(bricks);

        _namedGroupPalettes = DeterministicPalette.GetPositionalPaletteSet(
            regexGraph.NamedGroupFlatGraph.Values,
            positionalOverrideColors: HexColor.WhiteSmoke);

        _bookendRenderer = new GroupBookendCommentRenderer(SpanFromBrick, _metrics.MaxCommentLength);
    }

    /// <summary>Renders every brick in <paramref name="bricks"/> into its own <see cref="SmartLine"/>, in order.</summary>
    public static List<SmartLine> Render(List<RegexBrick> bricks, RegexGraph regexGraph) =>
        new SmartLineRenderer(bricks, regexGraph).RenderLines();

    List<SmartLine> RenderLines()
    {
        List<SmartLine> lines = [];

        foreach (var brick in _bricks)
        {
            if (brick is RegexBrickGroupClose)
                _wallTracker.Pop();

            List<SmartSpan> spans = [SpanFromBrick(brick, BuildRegexColumn(brick)), BuildCommentSeparatorSpan()];
            spans.AddRange(BuildCommentSpans(brick));

            lines.Add(new(spans));

            if (brick is RegexBrickGroupOpen groupOpen)
                _wallTracker.Push(BuildLeftWallSpan(groupOpen));
        }

        return lines;
    }

    /// <summary>The brick's regex text, indented to its nesting depth and padded out to the shared comment column.</summary>
    string BuildRegexColumn(RegexBrick brick)
    {
        var indented = string.Empty.PadLeft(brick.NestedDepth * SmartRegexStaticRules.IndentSpaces) + brick.RegexFormatted;
        return indented.PadRight(_metrics.CommentSeparatorColumn);
    }

    static SmartSpan BuildCommentSeparatorSpan() =>
        new(SmartRegexStaticRules.CommentBorderLineWithBuffer, "", DeterministicPalette.GetStaticPalette(HexColor.DarkSlateGrey));

    /// <summary>The comment-column spans for a brick: enclosing left walls, the comment content itself, then mirrored right walls.</summary>
    List<SmartSpan> BuildCommentSpans(RegexBrick brick)
    {
        List<SmartSpan> spans = [.. _wallTracker.LeftWalls];

        spans.AddRange(brick is RegexBrickGroupBookend bookend
            ? _bookendRenderer.Render(bookend)
            : [BuildRegularCommentSpan(brick)]);

        spans.AddRange(_wallTracker.RightWalls);

        return spans;
    }

    /// <summary>Center- or right-padded comment text for a non-bookend brick, sized to the shared comment box width.</summary>
    SmartSpan BuildRegularCommentSpan(RegexBrick brick)
    {
        var availableWidth = _metrics.MaxCommentLength - CommentBoxMetrics.GetGroupBoxPaddingCount(brick);

        var content = brick switch
        {
            RegexBrickValue or RegexBrickOmittedCount or RegexBrickSynonymSectionHeader =>
                SmartRegexStaticRules.CenterPad(brick.CommentFormatted, availableWidth),
            RegexBrickSynonymSectionFooter =>
                brick.CommentFormatted.PadRight(availableWidth, RegexBrickSynonymSectionFooter.DividerChar),
            _ => brick.CommentFormatted.PadRight(availableWidth),
        };

        return SpanFromBrick(brick, content);
    }

    SmartSpan BuildLeftWallSpan(RegexBrickGroupOpen groupOpen)
    {
        var boxChars = SmartRegexStaticRules.GetBoxCharsForBookendBrick(groupOpen);
        var wall = $"{boxChars.Vertical}{string.Empty.PadLeft(SmartRegexStaticRules.GroupWallInnerBuffer)}";
        return SpanFromBrick(groupOpen, wall);
    }

    SmartSpan SpanFromBrick(RegexBrick brick, string content) =>
        new(content, brick.FullyQualifiedName, _namedGroupPalettes[brick.NamedGroupParent]);
}
