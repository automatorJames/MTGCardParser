namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Renders an already-formatted brick sequence (see <see cref="RegexBrickFormattingPipeline"/>) into
/// <see cref="SmartLine"/>s: one line per brick, pairing its padded regex text with a colored, box-drawn
/// comment. Delegates group-box corner/padding-line rendering to <see cref="GroupBookendCommentRenderer"/>
/// and nested-wall bookkeeping to <see cref="GroupWallSpanTracker"/>; this class owns per-line assembly,
/// named-group palette lookup (the coloring fallback for bricks with no more specific role), and classifying
/// bricks into the <see cref="RegexSpanKind"/> roles that <see cref="SmartSpanControlPanel"/> knows how
/// to color. Replaces what used to be the monolithic <c>SmartLineFactory</c>.
/// </summary>
public class SmartLineRenderer
{
    static readonly Regex _spaceRunPattern = new("( +)", RegexOptions.Compiled);

    readonly List<RegexBrick> _bricks;
    readonly Dictionary<NamedGroupNode, SpanStylePalette> _namedGroupPalettes;
    readonly Dictionary<NamedGroupNode, double> _namedGroupHueDegrees;
    readonly Dictionary<NamedGroupNode, bool> _namedGroupIsGrayscale;
    readonly CommentBoxMetrics _metrics;
    readonly GroupWallSpanTracker _wallTracker = new();
    readonly GroupBookendCommentRenderer _bookendRenderer;

    SmartLineRenderer(List<RegexBrick> bricks, RegexGraph regexGraph)
    {
        _bricks = bricks;
        _metrics = new CommentBoxMetrics(bricks);

        // Seed with regexGraph's own declared named groups first, in their normal declaration order —
        // this includes its (never rendered as a box) transparent root, so the reserved neutral color at
        // position 0 stays reserved even for a graph with no bricks that reference the root directly, and
        // every other type's own page keeps exactly the coloring it always had. Then append any further
        // named groups that only show up via a dynamic capture's resolved sub-type — spliced into this
        // same brick sequence by DynamicSectionBuilder — in first-appearance order, so those join the same
        // rainbow instead of each restarting their own.
        var namedGroupsInDisplayOrder = regexGraph.NamedGroupFlatGraph.Values
            .Concat(bricks.Select(x => x.NamedGroupParent).Where(x => x is not null))
            .Distinct()
            .ToList();

        var namedGroupHexPalettes = DeterministicPalette
            .GetPositionalPaletteSet(namedGroupsInDisplayOrder, positionalOverrideColors: HexColor.Silver);

        _namedGroupPalettes = namedGroupHexPalettes.ToDictionary(x => x.Key, x => SpanStylePalette.FromHexPalette(x.Value));
        _namedGroupHueDegrees = namedGroupHexPalettes.ToDictionary(x => x.Key, x => DeterministicPalette.HexToHue(x.Value.Normal) * 360.0);
        _namedGroupIsGrayscale = namedGroupHexPalettes.ToDictionary(x => x.Key, x => HslMath.IsGrayscale(x.Value.Normal));

        _bookendRenderer = new GroupBookendCommentRenderer(SpanFromBrick, _metrics.MaxCommentLength);
    }

    /// <summary>Renders every brick in <paramref name="bricks"/> into its own <see cref="SmartLine"/>, in order.</summary>
    public static List<SmartLine> Render(List<RegexBrick> bricks, RegexGraph regexGraph) =>
        new SmartLineRenderer(bricks, regexGraph).RenderLines();

    /// <summary>
    /// Renders <paramref name="bricks"/> (typically <see cref="BuiltRegex.Bricks"/>, the raw, unranked/
    /// unfiltered sequence) as a single line of its own raw <see cref="RegexBrick.Regex"/> text, colored
    /// per named group the same way <see cref="Render"/> colors an ordinary brick with no more specific
    /// <see cref="RegexSpanKind"/> - just without the padding, box comments, or enum member ranking that a
    /// formatted line carries.
    /// </summary>
    public static SmartLine RenderMinifiedLine(List<RegexBrick> bricks, RegexGraph regexGraph)
    {
        var renderer = new SmartLineRenderer(bricks, regexGraph);
        return new(bricks.Select(brick => renderer.SpanFromBrick(brick, brick.Regex)).ToList());
    }

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
        var indent = string.Empty.PadLeft(SmartRegexStaticRules.GetIndentSpaces(brick));
        var indented = indent + brick.RegexFormatted;
        var trailingPad = string.Empty.PadRight(Math.Max(0, _metrics.CommentSeparatorColumn - indented.Length));

        if (brick is RegexBrickValue member)
        {
            return
            [
                SpanFromBrick(brick, indent + member.JoinerRegexFormatted, RegexSpanKind.RegexEnumMemberJoiner),
                SpanFromBrick(brick, member.MemberRegexFormatted + trailingPad, RegexSpanKind.RegexEnumMember),
            ];
        }

        if (brick is RegexBrickJoiner)
        {
            var kind = brick.RegexFormatted == "[ ]" ? RegexSpanKind.RegexConnectiveSpace : RegexSpanKind.RegexJoiner;
            return [SpanFromBrick(brick, indented + trailingPad, kind)];
        }

        if (brick.Parent is TextNode)
            return BuildLiteralMatchSpans(brick, indent, trailingPad);

        return [SpanFromBrick(brick, indented + trailingPad)];
    }

    /// <summary>
    /// Splits a literal-match brick's text into alternating word/space-run spans, so plain spaces embedded
    /// in a multi-word literal (e.g. "until end of turn") get the same <see cref="RegexSpanKind.RegexConnectiveSpace"/>
    /// treatment as a dedicated "[ ]" joiner brick — there's no strongly-typed node for "the space in the
    /// middle of a literal phrase", so this isolates it with a regex split instead.
    /// </summary>
    List<SmartSpan> BuildLiteralMatchSpans(RegexBrick brick, string indent, string trailingPad)
    {
        var text = brick.RegexFormatted;

        if (text == "[ ]")
            return [SpanFromBrick(brick, indent + text + trailingPad, RegexSpanKind.RegexConnectiveSpace)];

        var fragments = _spaceRunPattern.Split(text).Where(x => x.Length > 0).ToList();

        if (fragments.Count == 0)
            return [SpanFromBrick(brick, indent + trailingPad)];

        List<SmartSpan> spans = [];

        for (int i = 0; i < fragments.Count; i++)
        {
            var fragment = fragments[i];
            var kind = fragment.Trim().Length == 0 ? RegexSpanKind.RegexConnectiveSpace : RegexSpanKind.RegexLiteralMatch;
            var prefix = i == 0 ? indent : "";
            var suffix = i == fragments.Count - 1 ? trailingPad : "";
            spans.Add(SpanFromBrick(brick, prefix + fragment + suffix, kind));
        }

        return spans;
    }

    SmartSpan BuildCommentSeparatorSpan(RegexBrick brick) =>
        new(SmartRegexStaticRules.CommentBorderLineWithBuffer, "", ResolveRolePalette(brick, RegexSpanKind.RegexCommentSeparator), RegexSpanKind.RegexCommentSeparator);

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

    /// <summary>Center- or right-padded comment span(s) for a non-bookend brick, sized to the shared comment box width. An enum member row (standalone or synonym-group header) splits into its name and occurrence-count fields; the other synthesized Presentation brick kinds get their own dedicated role.</summary>
    List<SmartSpan> BuildRegularCommentSpans(RegexBrick brick)
    {
        // Use the wall tracker's live depth rather than brick.NestedDepth: synthetic bricks (e.g. blank
        // separator lines) don't carry an accurate NestedDepth of their own, but the tracker always
        // reflects exactly how many group-box walls are actually open at this point in rendering.
        var availableWidth = _metrics.MaxCommentLength - CommentBoxMetrics.GetGroupBoxPaddingCount(_wallTracker.Depth, isBookend: false);

        if (brick is RegexBrickValue member)
            return BuildNameCountSpans(brick, member.NameCommentFormatted, member.CountCommentFormatted, availableWidth);

        if (brick is RegexBrickSynonymSectionHeader header)
            return BuildNameCountSpans(brick, header.NameCommentFormatted, header.CountCommentFormatted, availableWidth);

        var (content, kind) = brick switch
        {
            RegexBrickOmittedCount =>
                (SmartRegexStaticRules.CenterPad(brick.CommentFormatted, availableWidth), (RegexSpanKind?)RegexSpanKind.CommentOmittedCount),
            RegexBrickSynonymSectionFooter =>
                (brick.CommentFormatted.PadRight(availableWidth, RegexBrickSynonymSectionFooter.DividerChar), (RegexSpanKind?)RegexSpanKind.CommentEnumMemberSynonymFooter),
            RegexBrickJoiner =>
                (brick.CommentFormatted.PadRight(availableWidth), (RegexSpanKind?)RegexSpanKind.CommentJoiner),
            _ when brick.Parent is TextNode =>
                (brick.CommentFormatted.PadRight(availableWidth), (RegexSpanKind?)RegexSpanKind.CommentLiteralMatch),
            _ =>
                (brick.CommentFormatted.PadRight(availableWidth), (RegexSpanKind?)null),
        };

        return [SpanFromBrick(brick, content, kind)];
    }

    /// <summary>Splits a "Name : count" comment into its name and count spans, sharing the same two roles whether <paramref name="brick"/> is a standalone member row or a synonym-group header.</summary>
    List<SmartSpan> BuildNameCountSpans(RegexBrick brick, string nameCommentFormatted, string countCommentFormatted, int availableWidth)
    {
        var (leftPad, rightPad) = SmartRegexStaticRules.CenterPadSplit(brick.CommentFormatted, availableWidth);

        return
        [
            SpanFromBrick(brick, leftPad + nameCommentFormatted, RegexSpanKind.CommentEnumMemberName),
            SpanFromBrick(brick, countCommentFormatted + rightPad, RegexSpanKind.CommentEnumMemberOccurrenceCount),
        ];
    }

    SmartSpan BuildLeftWallSpan(RegexBrickGroupOpen groupOpen)
    {
        var boxChars = SmartRegexStaticRules.GetBoxCharsForBookendBrick(groupOpen);
        var wall = $"{boxChars.Vertical}{string.Empty.PadLeft(SmartRegexStaticRules.GroupWallInnerBuffer)}";
        return SpanFromBrick(groupOpen, wall, RegexSpanKind.CommentGroupBorderWall);
    }

    /// <summary>Builds one colored span for <paramref name="brick"/>. With no <paramref name="kind"/>, falls back to the brick's enclosing named group's rainbow color (the older, coarser-grained coloring); with a <paramref name="kind"/>, colors it via <see cref="SmartSpanControlPanel"/> instead.</summary>
    SmartSpan SpanFromBrick(RegexBrick brick, string content, RegexSpanKind? kind = null) =>
        new(content, brick.FullyQualifiedName, ResolvePalette(brick, kind), kind);

    /// <summary>
    /// Resolves the palette for a role-tagged span: the role's saturation/brightness knobs, applied to this
    /// brick's own positional rainbow hue — see <see cref="SmartSpanControlPanel"/>.
    /// </summary>
    SpanStylePalette ResolveRolePalette(RegexBrick brick, RegexSpanKind kind) =>
        SmartSpanControlPanel.Resolve(
            kind,
            _namedGroupHueDegrees[brick.NamedGroupParent],
            forceGrayscale: _namedGroupIsGrayscale[brick.NamedGroupParent]);

    SpanStylePalette ResolvePalette(RegexBrick brick, RegexSpanKind? kind) =>
        kind is { } concreteKind ? ResolveRolePalette(brick, concreteKind) : _namedGroupPalettes[brick.NamedGroupParent];
}
