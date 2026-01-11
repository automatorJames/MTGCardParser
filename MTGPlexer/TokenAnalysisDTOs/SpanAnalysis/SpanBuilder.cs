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
            IsCollapsed = false,
            OriginalFullText = fullText,
            RootToken = root,
            CardName = cardName,
            ClauseIndex = clauseIndex
        };
    }

    private static SpanNode BuildNode(PropertyCapture prop, SpanContext ctx)
    {
        return prop.Value switch
        {
            TokenUnitOneOf val => BuildTokenUnitOneOfBranch(val, prop, ctx),
            TokenUnitDistilled val => BuildDistilledBranch(val, prop, ctx),
            TokenUnit val => BuildTokenUnitBranch(val, prop, ctx),
            ManyOf val => BuildManyOfBranch(val, prop, ctx),
            CompoundOf val => BuildCompoundOfBranch(val, prop, ctx),
            OneOf val => BuildOneOfBranch(val, prop, ctx),
            DynamicCapture val => BuildDynamicBranch(val, prop, ctx),
            PlaceholderCapture val => BuildLeaf(prop, ctx, val.Text, "placeholder", TokenAnalysisElementType.PlaceholderLeaf),
            bool val => BuildLeaf(prop, ctx, val.ToString().ToLower(), "bool", TokenAnalysisElementType.BoolLeaf),

            _ when prop.RegexPropInfo.RegexPropType == RegexPropType.Enum
                => BuildLeaf(prop, ctx, prop.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.EnumLeaf),

            _ => throw new InvalidOperationException($"Unsupported: {prop.Value?.GetType().Name}")
        };
    }

    private static SpanNode BuildTokenUnitBranch(TokenUnit tokenUnit, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.RegexPropInfo.Name);
        var children = tokenUnit.IndexedPropertyCaptures.Select(p => BuildNode(p, ctx.ClearNameChain())).ToList();
        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitBranch, children, ctx);
    }

    private static SpanNode BuildTokenUnitOneOfBranch(TokenUnitOneOf tokenUnitOneOf, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.RegexPropInfo.Name);
        var inner = tokenUnitOneOf.IndexedPropertyCaptures.Single();
        var childNode = BuildNode(inner, ctx.ClearNameChain());
        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.OneOfItemBranch, new List<SpanNode> { childNode }, ctx);
    }

    private static SpanNode BuildDynamicBranch(DynamicCapture dynamicCapture, PropertyCapture prop, SpanContext ctx)
    {
        // 1. Prepare context for child: Clear inherited prefixes, but push the Type as a Suffix
        // This ensures when the child calls FormatName("Action"), it gets "Action: Specific Action"
        var typeName = dynamicCapture.ValueObject.GetType().Name;
        var childCtx = ctx.ClearNameChain().PushSuffix(typeName);

        SpanNode innerNode = dynamicCapture.ValueObject switch
        {
            TokenUnitOneOf val => BuildTokenUnitOneOfBranch(val, prop, childCtx),
            TokenUnit val => BuildTokenUnitBranch(val, prop, childCtx),
            _ => BuildLeaf(prop, childCtx, dynamicCapture.ValueObject.ToString()!, "enum", TokenAnalysisElementType.DynamicCaptureItemLeaf)
        };

        // 2. Return the branch. The branch name itself is just the prop name (e.g. "Action")
        // since it will be collapsed in the GUI anyway.
        return CreateBranch(prop.Capture, prop.RegexPropInfo.Name, prop.CaptureGroupPropPath,
                            TokenAnalysisElementType.DynamicCaptureItemBranch,
                            new List<SpanNode> { innerNode }, ctx, forceCollapse: true);
    }

    private static SpanNode BuildManyOfBranch(ManyOf manyOf, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.RegexPropInfo.Name);
        var childCtx = ctx.ClearNameChain();
        var children = new List<SpanNode>();

        for (int i = 0; i < manyOf.ItemObjects.Count; i++)
        {
            var item = manyOf.ItemObjects[i];
            var itemPath = new CaptureGroupPropPath(prop.CaptureGroupPropPath + $"[{i}]");
            var itemLabel = $"#{i + 1}";

            if (manyOf.ManyItemVariant == CaptureTypeVariant.TokenUnit && item.ItemObject is TokenUnit tu)
            {
                var itemCtx = childCtx.PushName(itemLabel);
                var innerTU = BuildTokenUnitBranch(tu, item.Capture, itemPath, itemCtx);
                children.Add(CreateBranch(item.Capture, itemLabel, itemPath, TokenAnalysisElementType.ManyOfItemBranch, new List<SpanNode> { innerTU }, ctx));
            }
            else
            {
                children.Add(BuildLeaf(item.Capture, itemLabel, itemPath, ctx, item.ItemObject.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.ManyOfItemLeaf));
            }
        }

        if (manyOf.Conjunction != null)
        {
            var conjPath = new CaptureGroupPropPath(prop.CaptureGroupPropPath.PropPath.Dot(nameof(ManyOf.Conjunction)));
            var conjunctionLeaf = BuildLeaf(manyOf.ConjunctionCapture, nameof(ManyOf.Conjunction), conjPath, ctx, manyOf.Conjunction.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.ConjunctionLeaf);
            children.Add(conjunctionLeaf);
        }

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.ManyOfBranch, children, ctx);
    }

    private static SpanNode BuildCompoundOfBranch(CompoundOf compoundOf, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.RegexPropInfo.Name);
        var childCtx = ctx.ClearNameChain();
        var children = new List<SpanNode>();

        for (int i = 0; i < compoundOf.ItemObjects.Count; i++)
        {
            var item = compoundOf.ItemObjects[i];
            var itemPath = new CaptureGroupPropPath(prop.CaptureGroupPropPath + $"[{i}]");
            var itemLabel = $"#{i + 1}";

            if (compoundOf.CaptureTypeVariant == CaptureTypeVariant.TokenUnit && item.ItemObject is TokenUnit tu)
            {
                var itemCtx = childCtx.PushName(itemLabel);
                var innerTU = BuildTokenUnitBranch(tu, item.Capture, itemPath, itemCtx);
                children.Add(CreateBranch(item.Capture, itemLabel, itemPath, TokenAnalysisElementType.CompoundOfItemBranch, new List<SpanNode> { innerTU }, ctx));
            }
            else
            {
                children.Add(BuildLeaf(item.Capture, itemLabel, itemPath, ctx, item.ItemObject.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.CompoundOfItemLeaf));
            }
        }

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.CompoundOfBranch, children, ctx);
    }

    private static SpanNode BuildOneOfBranch(OneOf oneOf, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.RegexPropInfo.Name);
        var childCtx = ctx.ClearNameChain();
        var children = new List<SpanNode>();
        var itemLabel = oneOf.ItemObject.RegexPropInfo.Name;
        var itemPath = prop.CaptureGroupPropPath.Append(itemLabel);

        if (oneOf.ItemObject.ItemObject is TokenUnit tokenUnit)
        {
            var itemCtx = childCtx.PushName(prop.RegexPropInfo.Name);
            var innerTU = BuildTokenUnitBranch(tokenUnit, oneOf.ItemObject.Capture, itemPath, itemCtx);
            children.Add(CreateBranch(oneOf.ItemObject.Capture, itemLabel, itemPath, TokenAnalysisElementType.CompoundOfItemBranch, new List<SpanNode> { innerTU }, ctx));
        }
        else
        {
            children.Add(BuildLeaf(oneOf.ItemObject.Capture, itemLabel, itemPath, ctx, oneOf.ItemObject.ItemObject.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.OneOfItemLeaf));
        }

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.OneOfItemBranch, children, ctx);
    }



    private static SpanNode BuildDistilledBranch(TokenUnitDistilled tokenUnitDistilled, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.RegexPropInfo.Name);
        var childCtx = ctx.ClearNameChain();

        var children = tokenUnitDistilled.IndexedPropertyCaptures
            .Where(p => !tokenUnitDistilled.DistilledVals.ContainsKey(p))
            .Select(p => BuildNode(p, childCtx))
            .ToList();

        foreach (var (placeholder, vals) in tokenUnitDistilled.DistilledVals)
        {
            var subLeaves = vals.Select(v => new SpanSubLeaf
            {
                Name = v.Key.Name,
                TerminalValString = v.Value.ToString()!.ToLower(),
                TerminalType = "distilled",
                ElementType = TokenAnalysisElementType.DistilledValueSubLeaf,
                Start = placeholder.Capture.Index,
                End = placeholder.Capture.Index + placeholder.Capture.Length,
                Length = placeholder.Capture.Length,
                CaptureTextOriginal = placeholder.Capture.Value,
                CapturePath = new(ctx.PathPrefix + placeholder.CaptureGroupPropPath)
            }).Cast<SpanNode>().ToList();

            var childBranch = CreateBranch(
                placeholder.Capture,
                placeholder.RegexPropInfo.Name,
                placeholder.CaptureGroupPropPath,
                TokenAnalysisElementType.TokenUnitDistilledBranch,
                subLeaves,
                ctx);

            children.Add(childBranch);
        }

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitDistilledBranch, children, ctx);
    }

    private static SpanNode BuildTokenUnitBranch(TokenUnit tu, Capture cap, CaptureGroupPropPath path, SpanContext ctx)
    {
        var name = ctx.FormatName(tu.Type.Name);
        var children = tu.IndexedPropertyCaptures.Select(p => BuildNode(p, ctx.ClearNameChain())).ToList();
        return CreateBranch(cap, name, path, TokenAnalysisElementType.TokenUnitBranch, children, ctx);
    }

    private static SpanBranch CreateBranch(Capture cap, string name, CaptureGroupPropPath path, TokenAnalysisElementType type, List<SpanNode> children, SpanContext ctx, bool? forceCollapse = null)
    {
        return new SpanBranch
        {
            Name = name,
            CapturePath = new(ctx.PathPrefix + path),
            Start = cap.Index,
            Length = cap.Length,
            End = cap.Index + cap.Length,
            CaptureTextOriginal = ctx.FullText.Substring(cap.Index, cap.Length),
            ElementType = type,
            Children = children,
            // Use the override if provided, otherwise fallback to the automatic calculation
            IsCollapsed = forceCollapse ?? SpanBranch.CalculateIsCollapsed(children)
        };
    }

    private static SpanLeaf BuildLeaf(PropertyCapture prop, SpanContext ctx, string val, string typeName, TokenAnalysisElementType type) =>
        BuildLeaf(prop.Capture, prop.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence), prop.CaptureGroupPropPath, ctx, val, typeName, type);

    private static SpanLeaf BuildLeaf(Capture cap, string name, CaptureGroupPropPath path, SpanContext ctx, string val, string typeName, TokenAnalysisElementType type)
    {
        return new SpanLeaf
        {
            Name = name,
            CapturePath = new(ctx.PathPrefix + path),
            Start = cap.Index,
            Length = cap.Length,
            End = cap.Index + cap.Length,
            CaptureTextOriginal = ctx.FullText.Substring(cap.Index, cap.Length),
            ElementType = type,
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
    CompoundOfBranch, // Overall CompoundOf container, parent to items
    CompoundOfItemBranch,
    DynamicCaptureBranch,
    DynamicCaptureItemBranch,

    EnumLeaf,
    BoolLeaf,
    OneOfItemLeaf,
    ManyOfItemLeaf,
    CompoundOfItemLeaf,
    PlaceholderLeaf,
    PlaceholderPrecursorLeaf,
    DynamicCaptureItemLeaf,
    ConjunctionLeaf,

    DistilledValueSubLeaf,
}