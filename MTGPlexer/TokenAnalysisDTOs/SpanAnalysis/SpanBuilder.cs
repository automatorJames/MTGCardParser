namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

public static class SpanBuilder
{
    public static SpanRoot Create(TokenUnit root, string fullText, string cardName, int clauseIndex)
    {
        var prefix = $"{cardName.Replace(' ', '_')}-line[{clauseIndex}]-index[{root.Match.RootMatch.Index}]-";
        var ctx = new SpanContext(fullText, prefix);

        return new SpanRoot
        {
            Name = root.Type.Name.ToFriendlyCase(TitleDisplayOption.Title),
            CapturePath = new(prefix + root.Match.CapturePath),
            CaptureTextOriginal = fullText.Substring(root.Match.RootMatch.Index, root.Match.RootMatch.Length),
            Start = root.Match.RootMatch.Index,
            End = root.Match.RootMatch.End,
            Length = root.Match.RootMatch.Length,
            ElementType = root is DefaultUnmatchedString ? TokenAnalysisElementType.UnmatchedTokenUnitRoot : TokenAnalysisElementType.TokenUnitRoot,
            Children = root.PropertyCaptures.Select(p => BuildNode(p, ctx)).ToList(),
            IsCollapsed = false,
            OriginalFullText = fullText,
            RootToken = root,
            CardName = cardName,
            ClauseIndex = clauseIndex
        };
    }

    private static SpanNode BuildNode(PropertyCapture prop, SpanContext ctx)
    {
        return prop.TemplatePropInfo.TemplatePropType switch
        {
            TemplatePropType.TokenUnit => BuildTokenUnitBranch((TokenUnit)prop.Value, prop, ctx),
            TemplatePropType.TokenUnitOneOf => BuildTokenUnitOneOfBranch((TokenUnitOneOf)prop.Value, prop, ctx),
            TemplatePropType.Dynamic => BuildDynamicBranch((DynamicOf)prop.Value, prop, ctx),
            TemplatePropType.DistilledValue => BuildDistilledBranch((TokenUnitDistilled)prop.Value, prop, ctx),
            TemplatePropType.ManyOf or
            TemplatePropType.CompoundOf or
            TemplatePropType.OneOf or
            TemplatePropType.OptionalOf => BuildXOfBranch((XOf)prop.Value, prop, ctx),

            TemplatePropType.Placeholder => CreateLeaf(prop, ctx, ((PlaceholderCapture)prop.Value).Text, "placeholder", TokenAnalysisElementType.PlaceholderLeaf),
            TemplatePropType.Bool => CreateLeaf(prop, ctx, prop.Value.ToString()!.ToLower(), "bool", TokenAnalysisElementType.BoolLeaf),
            TemplatePropType.Enum => CreateLeaf(prop, ctx, prop.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.EnumLeaf),

            _ => throw new InvalidOperationException($"Unsupported PropType: {prop.TemplatePropInfo.TemplatePropType}")
        };
    }

    private static SpanBranch BuildTokenUnitBranch(TokenUnit tu, PropertyCapture prop, SpanContext ctx)
    {
        // 1. If we are Apple (Identity) filling the Fruit (Role) slot, we use the identity.
        // FormatName will prepend "Fruit" only if "Fruit" was pushed by a collapsed parent.
        var name = ctx.FormatName(tu.Type.Name);

        // 2. IMPORTANT: Since this branch is VISIBLE, its children do not need to repeat
        // its name. We clear the context for the next recursive level.
        var children = tu.PropertyCaptures.Select(p => BuildNode(p, ctx.Clear())).ToList();

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitBranch, children, ctx);
    }

    private static SpanBranch BuildTokenUnitOneOfBranch(TokenUnitOneOf tuOneOf, PropertyCapture prop, SpanContext ctx)
    {
        // This is a wrapper for Buff. We push "Buff" into the context so the 
        // resolved variant (CanAttack...) knows its role.
        var childCtx = ctx.PushName(prop.TemplatePropInfo.Name);
        var innerProp = tuOneOf.PropertyCaptures.Single();

        var children = new List<SpanNode> { BuildNode(innerProp, childCtx) };

        // We mark this collapsed so the UI skips the "Buff" header and shows "Buff: Can Attack..."
        return CreateBranch(prop.Capture, "one-of-wrapper", prop.CaptureGroupPropPath, TokenAnalysisElementType.OneOfItemBranch, children, ctx)
            with
        { IsCollapsed = true };
    }

    private static SpanBranch BuildDynamicBranch(DynamicOf dynamic, PropertyCapture prop, SpanContext ctx)
    {
        var typeName = dynamic.Item.Value.GetType().Name;

        // Dynamic properties are unique: the Identity (typeName) IS the interesting part.
        // We push the Role (e.g. "Action") and Suffix the Identity (e.g. "Draw").
        var childCtx = ctx.PushName(prop.TemplatePropInfo.Name).PushSuffix(typeName);
        var itemPath = prop.CaptureGroupPropPath.Append(typeName);

        SpanNode innerNode = dynamic.Item.Value switch
        {
            TokenUnit tu => (SpanNode)BuildTokenUnitBranch(tu, prop.Capture, itemPath, childCtx),
            _ => (SpanNode)CreateLeaf(prop.Capture, typeName, itemPath, childCtx, dynamic.Item.Value.ToString()!, "enum", TokenAnalysisElementType.DynamicCaptureItemLeaf)
        };

        return CreateBranch(prop.Capture, prop.TemplatePropInfo.Name, prop.CaptureGroupPropPath, TokenAnalysisElementType.DynamicCaptureItemBranch, [innerNode], ctx);
    }

    private static SpanBranch BuildXOfBranch(XOf xOf, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.TemplatePropInfo.Name);
        var children = new List<SpanNode>();

        // For items inside a collection, we usually clear context unless it's a OneOf.
        var childCtx = (xOf is OneOf or OptionalOf) ? ctx.PushName(prop.TemplatePropInfo.Name) : ctx.Clear();

        var items = GetXOfItems(xOf);
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var identifier = item.DistinguishingName ?? (xOf is OneOf or OptionalOf ? item.Type.Name : $"item[{i}]");
            var itemPath = prop.CaptureGroupPropPath.Append(identifier);
            var itemLabel = (xOf is OneOf or OptionalOf) ? prop.TemplatePropInfo.Name : $"#{i + 1}";

            if (item.Value is TokenUnit tu)
            {
                // We clear here because the BuildTokenUnitBranch inside will handle its own naming
                children.Add(BuildTokenUnitBranch(tu, item.Capture, itemPath, childCtx));
            }
            else
            {
                children.Add(CreateLeaf(item.Capture, itemLabel, itemPath, ctx, item.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", MapXOfElementType(xOf, false)));
            }
        }

        if (xOf is ManyOf { Conjunction: not null } manyOf)
        {
            var conjPath = prop.CaptureGroupPropPath.Append(nameof(ManyOf.Conjunction));
            children.Add(CreateLeaf(manyOf.ConjunctionCapture, nameof(ManyOf.Conjunction), conjPath, ctx, manyOf.Conjunction.Value.ToString()!.ToLower(), "enum", TokenAnalysisElementType.ConjunctionLeaf));
        }

        var branch = CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, MapXOfContainerType(xOf), children, ctx);

        if (xOf is OneOf or OptionalOf)
            return branch with { IsCollapsed = true };

        return branch;
    }

    private static SpanBranch BuildTokenUnitBranch(TokenUnit tu, ExtractedCapture cap, CaptureGroupPropPath path, SpanContext ctx)
    {
        var name = ctx.FormatName(tu.Type.Name);
        var children = tu.PropertyCaptures.Select(p => BuildNode(p, ctx.Clear())).ToList();
        return CreateBranch(cap, name, path, TokenAnalysisElementType.TokenUnitBranch, children, ctx);
    }

    private static SpanBranch BuildDistilledBranch(TokenUnitDistilled distilled, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.TemplatePropInfo.Name);
        var children = distilled.PropertyCaptures
            .Where(p => !distilled.DistilledVals.ContainsKey(p))
            .Select(p => BuildNode(p, ctx.Clear())).ToList();

        foreach (var (placeholder, vals) in distilled.DistilledVals)
        {
            var subLeaves = vals.Select(v => new SpanSubLeaf
            {
                Name = v.Key.Name,
                TerminalValString = v.Value.ToString()!.ToLower(),
                TerminalType = "distilled",
                ElementType = TokenAnalysisElementType.DistilledValueSubLeaf,
                Start = placeholder.Capture.Index,
                End = placeholder.Capture.End,
                Length = placeholder.Capture.Length,
                CaptureTextOriginal = placeholder.Capture.Value,
                CapturePath = new(ctx.PathPrefix + placeholder.CaptureGroupPropPath.Append(v.Key.Name))
            }).Cast<SpanNode>().ToList();

            children.Add(CreateBranch(placeholder.Capture, placeholder.TemplatePropInfo.Name, placeholder.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitDistilledBranch, subLeaves, ctx));
        }

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitDistilledBranch, children, ctx);
    }

    #region Helpers

    private static SpanBranch CreateBranch(ExtractedCapture cap, string name, CaptureGroupPropPath path, TokenAnalysisElementType type, List<SpanNode> children, SpanContext ctx) => new()
    {
        Name = name,
        CapturePath = new(ctx.PathPrefix + path),
        Start = cap.Index,
        Length = cap.Length,
        End = cap.End,
        CaptureTextOriginal = ctx.FullText.Substring(cap.Index, cap.Length),
        ElementType = type,
        Children = children,
        IsCollapsed = SpanBranch.CalculateIsCollapsed(children)
    };

    private static SpanLeaf CreateLeaf(PropertyCapture prop, SpanContext ctx, string val, string typeName, TokenAnalysisElementType type) =>
        CreateLeaf(prop.Capture, prop.TemplatePropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence), prop.CaptureGroupPropPath, ctx, val, typeName, type);

    private static SpanLeaf CreateLeaf(ExtractedCapture cap, string name, CaptureGroupPropPath path, SpanContext ctx, string val, string typeName, TokenAnalysisElementType type) => new()
    {
        Name = name,
        CapturePath = new(ctx.PathPrefix + path),
        Start = cap.Index,
        Length = cap.Length,
        End = cap.End,
        CaptureTextOriginal = ctx.FullText.Substring(cap.Index, cap.Length),
        ElementType = type,
        TerminalValString = val,
        TerminalType = typeName
    };

    private static List<PolyItemCapture> GetXOfItems(XOf xOf) => xOf switch
    {
        ManyOf m => m.Items,
        CompoundOf c => c.Items,
        OneOf o => [o.Item],
        OptionalOf p => [p.Item],
        _ => []
    };

    private static TokenAnalysisElementType MapXOfContainerType(XOf xOf) => xOf switch
    {
        ManyOf => TokenAnalysisElementType.ManyOfBranch,
        CompoundOf => TokenAnalysisElementType.CompoundOfBranch,
        _ => TokenAnalysisElementType.OneOfItemBranch
    };

    private static TokenAnalysisElementType MapXOfElementType(XOf xOf, bool isBranch) => xOf switch
    {
        ManyOf => isBranch ? TokenAnalysisElementType.ManyOfItemBranch : TokenAnalysisElementType.ManyOfItemLeaf,
        CompoundOf => isBranch ? TokenAnalysisElementType.CompoundOfItemBranch : TokenAnalysisElementType.CompoundOfItemLeaf,
        _ => isBranch ? TokenAnalysisElementType.CompoundOfItemBranch : TokenAnalysisElementType.OneOfItemLeaf
    };

    #endregion
}

public enum TokenAnalysisElementType
{
    UnmatchedTokenUnitRoot,
    TokenUnitRoot,
    TokenUnitBranch,
    TokenUnitOneOfBranch,
    TokenUnitDistilledBranch,
    OneOfItemBranch,
    OptionalOfItemBranch,
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