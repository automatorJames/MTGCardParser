namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// Factory service that builds a SpanAnalysis tree from a root TokenUnit.
/// Replaces the legacy precursor-node logic with a cleaner, single-pass recursive build.
/// </summary>
public static class SpanBuilder
{
    public static SpanRoot Create(TokenUnit root, string fullText, string cardName, int clauseIndex)
    {
        var prefix = $"{cardName.Replace(' ', '_')}-line[{clauseIndex}]-index[{root.Match.RegexMatch.Index}]-";
        var ctx = new SpanContext(fullText, prefix);

        var children = root.IndexedPropertyCaptures.Select(p => BuildNode(p, ctx)).ToList();

        return new SpanRoot
        {
            Name = root.Type.Name.ToFriendlyCase(TitleDisplayOption.Title),
            CapturePath = new(prefix + root.Match.CapturePath),
            CaptureTextOriginal = fullText.Substring(root.Match.RegexMatch.Index, root.Match.RegexMatch.Length),
            Start = root.Match.RegexMatch.Index,
            End = root.Match.RegexMatch.Index + root.Match.RegexMatch.Length,
            Length = root.Match.RegexMatch.Length,
            ElementType = root is DefaultUnmatchedString ? TokenAnalysisElementType.UnmatchedTokenUnitRoot : TokenAnalysisElementType.TokenUnitRoot,
            Children = children,
            Palette = TokenTypeRegistry.Palettes[root.Type],
            IsCollapsed = false,
            OriginalFullText = fullText,
            RootToken = root,
            CardName = cardName,
            ClauseIndex = clauseIndex
        };
    }

    private static SpanNode BuildNode(IndexedPropertyCapture prop, SpanContext ctx)
    {
        return prop.Value switch
        {
            TokenUnitOneOf val => BuildOneOfBranch(val, prop, ctx),
            TokenUnitDistilled val => BuildDistilledBranch(val, prop, ctx),
            TokenUnit val => BuildTokenUnitBranch(val, prop, ctx),
            ManyOf val => BuildManyOfBranch(val, prop, ctx),
            DynamicCapture val => BuildDynamicBranch(val, prop, ctx),
            PlaceholderCapture val => BuildLeaf(prop, ctx, val.Text, "placeholder", TokenAnalysisElementType.PlaceholderLeaf),
            bool val => BuildLeaf(prop, ctx, val.ToString().ToLower(), "bool", TokenAnalysisElementType.BoolLeaf),

            _ when prop.RegexPropInfo.RegexPropType == RegexPropType.Enum 
                => BuildLeaf(prop, ctx, prop.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.EnumLeaf),

            _ => throw new InvalidOperationException($"Unsupported: {prop.Value?.GetType().Name}")
        };
    }

    private static SpanNode BuildTokenUnitBranch(TokenUnit tu, IndexedPropertyCapture prop, SpanContext ctx)
    {
        var (name, childCtx) = ResolveNaming(prop, ctx);
        var children = tu.IndexedPropertyCaptures.Select(p => BuildNode(p, childCtx)).ToList();
        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitBranch, TokenTypeRegistry.Palettes[tu.Type], children, ctx);
    }

    private static SpanNode BuildOneOfBranch(TokenUnitOneOf oneOf, IndexedPropertyCapture prop, SpanContext ctx)
    {
        var (name, childCtx) = ResolveNaming(prop, ctx);
        var inner = oneOf.IndexedPropertyCaptures.Single();
        var childNode = BuildNode(inner, childCtx.ClearNameChain());
        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.OneOfItemBranch, TokenTypeRegistry.Palettes[oneOf.Type], new List<SpanNode> { childNode }, ctx);
    }

    private static SpanNode BuildDynamicBranch(DynamicCapture dyn, IndexedPropertyCapture prop, SpanContext ctx)
    {
        var (name, childCtx) = ResolveNaming(prop, ctx);
        var shiftedCtx = childCtx.WithOffset(prop.Capture.Index);

        SpanNode innerNode = dyn.ValueObject switch
        {
            TokenUnitOneOf val => BuildOneOfBranch(val, prop, shiftedCtx),
            TokenUnit val => BuildTokenUnitBranch(val, prop, shiftedCtx),
            _ => BuildLeaf(prop, shiftedCtx, dyn.ValueObject.ToString()!, "enum", TokenAnalysisElementType.DynamicCaptureItemLeaf)
        };

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.DynamicCaptureItemBranch, ctx.GetPalette(dyn), new List<SpanNode> { innerNode }, ctx);
    }

    private static SpanNode BuildManyOfBranch(ManyOf many, IndexedPropertyCapture prop, SpanContext ctx)
    {
        var (name, childCtx) = ResolveNaming(prop, ctx);
        var palette = TokenTypeRegistry.Palettes.TryGetValue(many.ItemType, out var p) ? p : ctx.GetPalette(many);
        var children = new List<SpanNode>();

        for (int i = 0; i < many.ItemObjects.Count; i++)
        {
            var item = many.ItemObjects[i];
            var itemPath = new CaptureGroupPropPath(prop.CaptureGroupPropPath + $"[{i}]");
            var itemLabel = $"#{i + 1}";

            if (many.ManyItemVariant == ManyItemVariant.TokenUnit && item.ItemObject is TokenUnit tu)
            {
                // Push name for prepending inside the item
                var itemCtx = childCtx.PushName(itemLabel);
                var innerTU = BuildTokenUnitBranch(tu, item.Capture, itemPath, itemCtx);
                children.Add(CreateBranch(item.Capture, itemLabel, itemPath, TokenAnalysisElementType.ManyOfItemBranch, palette, new List<SpanNode> { innerTU }, ctx));
            }
            else
            {
                children.Add(BuildLeaf(item.Capture, itemLabel, itemPath, i, ctx, item.ItemObject.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.ManyOfItemLeaf));
            }
        }

        if (many.Conjunction != null)
        {
            var conjPath = new CaptureGroupPropPath(prop.CaptureGroupPropPath.PropPath.Dot(nameof(ManyOf.Conjunction)));
            children.Add(BuildLeaf(many.ConjunctionCapture, nameof(ManyOf.Conjunction), conjPath, many.ItemObjects.Count, ctx, many.Conjunction.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.ConjunctionLeaf));
        }

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.ManyOfBranch, palette, children, ctx);
    }

    private static SpanNode BuildDistilledBranch(TokenUnitDistilled dist, IndexedPropertyCapture prop, SpanContext ctx)
    {
        var (name, childCtx) = ResolveNaming(prop, ctx);
        var children = dist.IndexedPropertyCaptures
            .Where(p => !dist.DistilledVals.ContainsKey(p))
            .Select(p => BuildNode(p, childCtx)).ToList();

        foreach (var (placeholder, vals) in dist.DistilledVals)
        {
            var subLeaves = vals.Select(v => new SpanSubLeaf
            {
                Name = v.Key.Name,
                TerminalValString = v.Value.ToString()!.ToLower(),
                TerminalType = "distilled",
                ElementType = TokenAnalysisElementType.DistilledValueSubLeaf,
                Start = placeholder.Capture.Index + ctx.AbsoluteOffset,
                End = placeholder.Capture.Index + placeholder.Capture.Length + ctx.AbsoluteOffset,
                CaptureTextOriginal = placeholder.Capture.Value,
                CapturePath = new(ctx.PathPrefix + placeholder.CaptureGroupPropPath)
            }).Cast<SpanNode>().ToList();

            var childBranch = CreateBranch(
                placeholder.Capture, 
                placeholder.RegexPropInfo.Name, 
                placeholder.CaptureGroupPropPath, 
                TokenAnalysisElementType.TokenUnitDistilledBranch,
                DeterministicPalette.GetFixedRainbowPalette(placeholder.Ordinal), 
                subLeaves, 
                ctx);

            children.Add(childBranch);
        }

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitDistilledBranch, TokenTypeRegistry.Palettes[dist.Type], children, ctx);
    }

    // --- Helpers and Overloads ---

    private static SpanNode BuildTokenUnitBranch(TokenUnit tu, Capture cap, CaptureGroupPropPath path, SpanContext ctx)
    {
        var (name, childCtx) = ResolveNaming(tu.Type.Name, ctx);
        var children = tu.IndexedPropertyCaptures.Select(p => BuildNode(p, childCtx)).ToList();
        return CreateBranch(cap, name, path, TokenAnalysisElementType.TokenUnitBranch, TokenTypeRegistry.Palettes[tu.Type], children, ctx);
    }

    private static (string Name, SpanContext NewCtx) ResolveNaming(IndexedPropertyCapture prop, SpanContext ctx) =>
        ResolveNaming(prop.RegexPropInfo.Name, ctx);

    private static (string Name, SpanContext NewCtx) ResolveNaming(string rawName, SpanContext ctx)
    {
        var friendly = rawName.ToFriendlyCase(TitleDisplayOption.Sentence);
        if (ctx.CurrentNameChain.Count > 0)
        {
            return ($"{string.Join(": ", ctx.CurrentNameChain)}: {friendly}", ctx.ClearNameChain());
        }
        return (friendly, ctx);
    }

    private static SpanBranch CreateBranch(Capture cap, string name, CaptureGroupPropPath path, TokenAnalysisElementType type, Palette palette, List<SpanNode> children, SpanContext ctx)
    {
        return new SpanBranch
        {
            Name = name,
            CapturePath = new(ctx.PathPrefix + path),
            Start = cap.Index + ctx.AbsoluteOffset,
            Length = cap.Length,
            End = cap.Index + cap.Length + ctx.AbsoluteOffset,
            CaptureTextOriginal = ctx.FullText.Substring(cap.Index + ctx.AbsoluteOffset, cap.Length),
            ElementType = type,
            Palette = palette,
            Children = children,
            IsCollapsed = SpanBranch.CalculateIsCollapsed(children)
        };
    }

    private static SpanLeaf BuildLeaf(IndexedPropertyCapture prop, SpanContext ctx, string val, string typeName, TokenAnalysisElementType type) =>
        BuildLeaf(prop.Capture, prop.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence), prop.CaptureGroupPropPath, prop.Ordinal, ctx, val, typeName, type);

    private static SpanLeaf BuildLeaf(Capture cap, string name, CaptureGroupPropPath path, int ordinal, SpanContext ctx, string val, string typeName, TokenAnalysisElementType type)
    {
        return new SpanLeaf
        {
            Name = name,
            CapturePath = new(ctx.PathPrefix + path),
            Start = cap.Index + ctx.AbsoluteOffset,
            Length = cap.Length,
            End = cap.Index + cap.Length + ctx.AbsoluteOffset,
            CaptureTextOriginal = ctx.FullText.Substring(cap.Index + ctx.AbsoluteOffset, cap.Length),
            ElementType = type,
            Palette = DeterministicPalette.GetFixedRainbowPalette(ordinal),
            TerminalValString = val,
            TerminalType = typeName
        };
    }
}
public enum TokenAnalysisElementType
{
    UnmatchedTokenUnitRoot,
    TokenUnitRoot,
    TokenUnitBranch,
    TokenUnitOneOfBranch,
    TokenUnitDistilledBranch,
    OneOfItemBranch,
    ManyOfBranch, // Overall ManyOf container, parent to Conjunction & items
    ManyOfItemBranch,
    DynamicCaptureBranch,
    DynamicCaptureItemBranch,

    EnumLeaf,
    BoolLeaf,
    OneOfItemLeaf,
    ManyOfItemLeaf,
    PlaceholderLeaf,
    PlaceholderPrecursorLeaf,
    DynamicCaptureItemLeaf,
    ConjunctionLeaf,

    DistilledValueSubLeaf,
}