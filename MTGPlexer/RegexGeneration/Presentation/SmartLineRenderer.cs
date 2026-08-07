namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Renders an already-formatted brick sequence (see <see cref="RegexBrickFormattingPipeline"/>) into
/// <see cref="SmartLine"/>s: one line per brick, pairing its padded regex text with a colored, box-drawn
/// comment. Delegates group-box corner/padding-line rendering to <see cref="GroupBookendCommentRenderer"/>
/// and nested-wall bookkeeping to <see cref="GroupWallSpanTracker"/>; this class owns per-line assembly,
/// named-group palette lookup (the coloring fallback for bricks with no more specific role), and classifying
/// bricks into the <see cref="TokenRegexSpanKind"/> roles that <see cref="SmartSpanColorPanel"/> knows how
/// to color. Replaces what used to be the monolithic <c>SmartLineFactory</c>.
/// </summary>
public class SmartLineRenderer
{
    static readonly Regex _spaceRunPattern = new("( +)", RegexOptions.Compiled);

    readonly List<RegexBrick> _bricks;
    readonly Dictionary<NamedGroupNode, SpanColorPalette> _namedGroupPalettes;
    readonly Dictionary<NamedGroupNode, double> _namedGroupHueDegrees;
    readonly CommentBoxMetrics _metrics;
    readonly GroupWallSpanTracker _wallTracker = new();
    readonly GroupBookendCommentRenderer _bookendRenderer;

    SmartLineRenderer(List<RegexBrick> bricks, RegexGraph regexGraph)
    {
        _bricks = bricks;
        _metrics = new CommentBoxMetrics(bricks);

        var namedGroupHexPalettes = DeterministicPalette
            .GetPositionalPaletteSet(regexGraph.NamedGroupFlatGraph.Values, positionalOverrideColors: HexColor.WhiteSmoke);

        _namedGroupPalettes = namedGroupHexPalettes.ToDictionary(x => x.Key, x => SpanColorPalette.FromHexPalette(x.Value));
        _namedGroupHueDegrees = namedGroupHexPalettes.ToDictionary(x => x.Key, x => DeterministicPalette.HexToHue(x.Value.Normal) * 360.0);

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

            List<SmartSpan> spans = [.. BuildRegexColumnSpans(brick), BuildCommentSeparatorSpan(brick)];
            spans.AddRange(BuildCommentSpans(brick));

            lines.Add(new(spans));

            if (brick is RegexBrickGroupOpen groupOpen)
                _wallTracker.Push(BuildLeftWallSpan(groupOpen));
        }

        return lines;
    }

    /// <summary>
    /// The brick's regex-column span(s): indented to its nesting depth and padded out to the shared comment
    /// column, same as before. Most brick kinds still render as a single span colored by their enclosing
    /// named group, but a few kinds get split into role-tagged sub-spans: an enum member row's leading
    /// joiner vs. its pattern text, and a literal-match brick's words vs. its connective spaces.
    /// </summary>
    List<SmartSpan> BuildRegexColumnSpans(RegexBrick brick)
    {
        var indent = string.Empty.PadLeft(brick.NestedDepth * SmartRegexStaticRules.IndentSpaces);
        var indented = indent + brick.RegexFormatted;
        var trailingPad = string.Empty.PadRight(Math.Max(0, _metrics.CommentSeparatorColumn - indented.Length));

        if (brick is RegexBrickValue member)
        {
            var memberKind = member.IsSynonymRow ? TokenRegexSpanKind.EnumMemberSynonymRegex : TokenRegexSpanKind.EnumMemberRegex;

            return
            [
                SpanFromBrick(brick, indent + member.JoinerRegexFormatted, TokenRegexSpanKind.EnumMemberJoiner),
                SpanFromBrick(brick, member.MemberRegexFormatted + trailingPad, memberKind),
            ];
        }

        if (brick is RegexBrickJoiner)
        {
            var kind = brick.RegexFormatted == "[ ]" ? TokenRegexSpanKind.ConnectiveSpace : TokenRegexSpanKind.RegexJoiner;
            return [SpanFromBrick(brick, indented + trailingPad, kind)];
        }

        if (brick.Parent is TextNode)
            return BuildLiteralMatchSpans(brick, indent, trailingPad);

        return [SpanFromBrick(brick, indented + trailingPad)];
    }

    /// <summary>
    /// Splits a literal-match brick's text into alternating word/space-run spans, so plain spaces embedded
    /// in a multi-word literal (e.g. "until end of turn") get the same <see cref="TokenRegexSpanKind.ConnectiveSpace"/>
    /// treatment as a dedicated "[ ]" joiner brick — there's no strongly-typed node for "the space in the
    /// middle of a literal phrase", so this isolates it with a regex split instead.
    /// </summary>
    List<SmartSpan> BuildLiteralMatchSpans(RegexBrick brick, string indent, string trailingPad)
    {
        var text = brick.RegexFormatted;

        if (text == "[ ]")
            return [SpanFromBrick(brick, indent + text + trailingPad, TokenRegexSpanKind.ConnectiveSpace)];

        var fragments = _spaceRunPattern.Split(text).Where(x => x.Length > 0).ToList();

        if (fragments.Count == 0)
            return [SpanFromBrick(brick, indent + trailingPad)];

        List<SmartSpan> spans = [];

        for (int i = 0; i < fragments.Count; i++)
        {
            var fragment = fragments[i];
            var kind = fragment.Trim().Length == 0 ? TokenRegexSpanKind.ConnectiveSpace : TokenRegexSpanKind.LiteralMatch;
            var prefix = i == 0 ? indent : "";
            var suffix = i == fragments.Count - 1 ? trailingPad : "";
            spans.Add(SpanFromBrick(brick, prefix + fragment + suffix, kind));
        }

        return spans;
    }

    SmartSpan BuildCommentSeparatorSpan(RegexBrick brick) =>
        new(SmartRegexStaticRules.CommentBorderLineWithBuffer, "", ResolveRolePalette(brick, TokenRegexSpanKind.RegexCommentSeparator));

    /// <summary>The comment-column spans for a brick: enclosing left walls, the comment content itself, then mirrored right walls.</summary>
    List<SmartSpan> BuildCommentSpans(RegexBrick brick)
    {
        List<SmartSpan> spans = [.. _wallTracker.LeftWalls];

        spans.AddRange(brick is RegexBrickGroupBookend bookend
            ? _bookendRenderer.Render(bookend)
            : BuildRegularCommentSpans(brick));

        spans.AddRange(_wallTracker.RightWalls);

        return spans;
    }

    /// <summary>Center- or right-padded comment span(s) for a non-bookend brick, sized to the shared comment box width. An enum member row splits into its name and occurrence-count fields; the other synthesized Presentation brick kinds get their own dedicated role.</summary>
    List<SmartSpan> BuildRegularCommentSpans(RegexBrick brick)
    {
        // Use the wall tracker's live depth rather than brick.NestedDepth: synthetic bricks (e.g. blank
        // separator lines) don't carry an accurate NestedDepth of their own, but the tracker always
        // reflects exactly how many group-box walls are actually open at this point in rendering.
        var availableWidth = _metrics.MaxCommentLength - CommentBoxMetrics.GetGroupBoxPaddingCount(_wallTracker.Depth, isBookend: false);

        if (brick is RegexBrickValue member)
        {
            var (leftPad, rightPad) = SmartRegexStaticRules.CenterPadSplit(brick.CommentFormatted, availableWidth);

            return
            [
                SpanFromBrick(brick, leftPad + member.NameCommentFormatted, TokenRegexSpanKind.EnumMemberName),
                SpanFromBrick(brick, member.CountCommentFormatted + rightPad, TokenRegexSpanKind.EnumMemberOccurrenceCount),
            ];
        }

        var (content, kind) = brick switch
        {
            RegexBrickOmittedCount =>
                (SmartRegexStaticRules.CenterPad(brick.CommentFormatted, availableWidth), (TokenRegexSpanKind?)TokenRegexSpanKind.OmittedCount),
            RegexBrickSynonymSectionHeader =>
                (SmartRegexStaticRules.CenterPad(brick.CommentFormatted, availableWidth), (TokenRegexSpanKind?)TokenRegexSpanKind.EnumMemberSynonymHeader),
            RegexBrickSynonymSectionFooter =>
                (brick.CommentFormatted.PadRight(availableWidth, RegexBrickSynonymSectionFooter.DividerChar), (TokenRegexSpanKind?)TokenRegexSpanKind.EnumMemberSynonymFooter),
            _ =>
                (brick.CommentFormatted.PadRight(availableWidth), (TokenRegexSpanKind?)null),
        };

        return [SpanFromBrick(brick, content, kind)];
    }

    SmartSpan BuildLeftWallSpan(RegexBrickGroupOpen groupOpen)
    {
        var boxChars = SmartRegexStaticRules.GetBoxCharsForBookendBrick(groupOpen);
        var wall = $"{boxChars.Vertical}{string.Empty.PadLeft(SmartRegexStaticRules.GroupWallInnerBuffer)}";
        return SpanFromBrick(groupOpen, wall, TokenRegexSpanKind.GroupBorderWall);
    }

    /// <summary>Builds one colored span for <paramref name="brick"/>. With no <paramref name="kind"/>, falls back to the brick's enclosing named group's rainbow color (the older, coarser-grained coloring); with a <paramref name="kind"/>, colors it via <see cref="SmartSpanColorPanel"/> instead.</summary>
    SmartSpan SpanFromBrick(RegexBrick brick, string content, TokenRegexSpanKind? kind = null) =>
        new(content, brick.FullyQualifiedName, ResolvePalette(brick, kind));

    /// <summary>
    /// Resolves the palette for a role-tagged span: the role's saturation/brightness knobs, applied to this
    /// brick's own positional rainbow hue unless the role (or its <see cref="CaptureNodeType"/>-specific
    /// override) declares a fixed hue of its own — see <see cref="SmartSpanColorPanel"/>.
    /// </summary>
    SpanColorPalette ResolveRolePalette(RegexBrick brick, TokenRegexSpanKind kind) =>
        SmartSpanColorPanel.Resolve(kind, _namedGroupHueDegrees[brick.NamedGroupParent], brick.NamedGroupParent?.NodeType);

    SpanColorPalette ResolvePalette(RegexBrick brick, TokenRegexSpanKind? kind) =>
        kind is { } concreteKind ? ResolveRolePalette(brick, concreteKind) : _namedGroupPalettes[brick.NamedGroupParent];
}
