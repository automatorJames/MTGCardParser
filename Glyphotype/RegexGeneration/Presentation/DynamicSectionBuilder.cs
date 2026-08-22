namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Builds the bricks that represent one dynamic group's resolved captures once formatting reaches that
/// group's opening brick: for each type resolved at runtime, a container bookended and commented the same
/// way any other nested <see cref="Glyph"/> group is, wrapping that type's own bricks — built from the
/// actual hydrated instances captured as that type, via the same <see cref="RegexBrickFormattingPipeline"/>
/// every type's own page uses, then depth-shifted to sit one level inside the container. Because these are
/// real bricks spliced into the outer sequence (not independently pre-rendered text), they share the outer
/// render's comment-column alignment, group-box walls, and positional rainbow coloring, and recurse
/// arbitrarily deep for free when a resolved type itself contains a nested dynamic group. When multiple
/// types were resolved for this group, their containers are separated by a pipe joiner — the same way any
/// other set of sibling alternatives is — since each is one branch of what this capture could resolve to.
/// When nothing was captured (e.g. a zero-occurrence type), falls back to rendering the group's raw bricks.
/// </summary>
internal class DynamicSectionBuilder
{
    /// <summary>Builds the full ordered sequence of embedded resolved-type sections for <paramref name="dynamicNode"/>.</summary>
    public List<RegexBrick> Build(DynamicGlyphNode dynamicNode, List<RegexBrick> allBricks, DynamicCaptureTraceSummary dynamicSummary, bool includeSupplementalLines, RegexDisplayMode displayMode)
    {
        if (dynamicSummary.ResolvedTypeGlyphs.Count == 0)
            return BuildFallbackBricks(dynamicNode, allBricks);

        var typeGroups = dynamicSummary.ResolvedTypeGlyphs
            .OrderByDescending(x => x.Value.Count)
            .ToList();

        List<RegexBrick> bricks = [];

        for (int i = 0; i < typeGroups.Count; i++)
        {
            var (type, glyphs) = typeGroups[i];

            if (i > 0)
            {
                if (includeSupplementalLines)
                    bricks.Add(new RegexBrickBlank(dynamicNode));

                bricks.Add(BuildPipeJoiner(dynamicNode));

                if (includeSupplementalLines)
                    bricks.Add(new RegexBrickBlank(dynamicNode));
            }

            bricks.AddRange(BuildResolvedTypeContainerBricks(dynamicNode, type, glyphs, includeSupplementalLines, displayMode));
        }

        return bricks;
    }

    /// <summary>A pipe joiner between two resolved types' containers, one level inside <paramref name="dynamicNode"/> — each is a sibling alternative of what this capture could resolve to.</summary>
    static RegexBrickJoiner BuildPipeJoiner(DynamicGlyphNode dynamicNode)
    {
        var joiner = new RegexBrickJoiner(dynamicNode, Joiner.Pipe);
        BrickCommentResolver.Apply(joiner);
        return joiner;
    }

    /// <summary>
    /// The group's raw, graph-produced bricks (its literal-match pattern and any inter-pattern joiners),
    /// with their display comments resolved, used when there's no captured data to summarize instead.
    /// </summary>
    static List<RegexBrick> BuildFallbackBricks(DynamicGlyphNode dynamicNode, List<RegexBrick> allBricks)
    {
        var rawBricks = allBricks
            .Where(x => x.NamedGroupParent == dynamicNode && (x.Parent is TextNode || x is RegexBrickJoiner))
            .ToList();

        foreach (var brick in rawBricks)
            BrickCommentResolver.Apply(brick);

        return rawBricks;
    }

    /// <summary>
    /// One resolved type's container: an open/close bookend pair labeled the same way a real nested token
    /// group is, one level inside <paramref name="dynamicNode"/>, wrapping that type's own fully-formatted
    /// bricks (from its actual captured instances) depth-shifted one level deeper still. Every brick's data
    /// path is rebased onto <paramref name="dynamicNode"/>'s, and the container itself is given the resolved
    /// type's own root as its coloring identity, so it reads (and colors) as a real nested group rather than
    /// as more of the enclosing dynamic group.
    /// </summary>
    static List<RegexBrick> BuildResolvedTypeContainerBricks(DynamicGlyphNode dynamicNode, Type resolvedType, List<Glyph> glyphs, bool includeSupplementalLines, RegexDisplayMode displayMode)
    {
        var resolvedGraph = GlyphTypeRegistry.RegexGraphs[resolvedType];
        var containerDepth = dynamicNode.Lineage.OfType<NamedGroupNode>().Count(x => !x.IsTransparentRoot);
        var typeLabel = CaptureNodeKind.Token.ToString().ToFriendlyCase(TitleDisplayOption.Lower);
        var friendlyName = resolvedType.Name.ToFriendlyCase();
        // Must match the prefix CaptureTrace.AdoptDynamicChildren rebases this same resolved type's real
        // captured descendants onto, so a data-path built from one lines up with a data-path built from
        // the other (e.g. MatchContentRenderer's spans hovering the right line here).
        var containerFullyQualifiedName = $"{dynamicNode.FullyQualifiedName}_{resolvedGraph.RootNode.FullyQualifiedName}";

        var open = new RegexBrickGroupOpen(dynamicNode, resolvedType.Name) { TypeLabel = typeLabel, CommentFormatted = typeLabel };
        var close = new RegexBrickGroupClose(dynamicNode, null) { NameText = friendlyName, CommentFormatted = friendlyName };

        foreach (var bookend in new RegexBrick[] { open, close })
        {
            // A self-referencing bookend's own NestedDepthModifer (-1) assumes it's opening/closing the
            // group it was built from (dynamicNode itself), landing its base depth one level shallower
            // than containerDepth. It's actually nested one level inside that group, so restore that level.
            bookend.OffsetNestedDepth(1);
            bookend.OverrideFullyQualifiedName(containerFullyQualifiedName);
            // Give the container its own coloring identity (the resolved type's own root) instead of
            // inheriting dynamicNode's — otherwise it renders the same color as the group it's nested in.
            bookend.OverrideNamedGroupParent(resolvedGraph.RootNode);
        }

        var content = RenderResolvedType(resolvedGraph, glyphs, includeSupplementalLines, displayMode);
        var resolvedRootFullyQualifiedName = resolvedGraph.RootNode.FullyQualifiedName;

        foreach (var brick in content)
        {
            brick.OffsetNestedDepth(containerDepth + 1);
            brick.OverrideFullyQualifiedName(containerFullyQualifiedName + brick.FullyQualifiedName[resolvedRootFullyQualifiedName.Length..]);
        }

        return [open, .. content, close];
    }

    /// <summary>
    /// Formats one resolved type's own bricks from the actual instances captured as that type — the same
    /// <see cref="RegexBrickFormattingPipeline"/> every registered type's own page runs — without rendering
    /// them yet, so they can be depth-shifted and spliced into the outer sequence for one shared render pass.
    /// </summary>
    static List<RegexBrick> RenderResolvedType(RegexGraph resolvedGraph, List<Glyph> glyphs, bool includeSupplementalLines, RegexDisplayMode displayMode)
    {
        var resolvedSummary = new GlyphOccurrenceSummary(resolvedGraph.RootGlyphType, glyphs.Select(g => new MatchOccurrence(null, g)));
        var pipeline = new RegexBrickFormattingPipeline(resolvedGraph, resolvedSummary, displayMode);

        return pipeline.Format(resolvedGraph.BuiltRegex.Bricks, includeSupplementalLines);
    }
}
