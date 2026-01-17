using MTGPlexer.TokenUnitComponents;

namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// Factory service that builds a SpanAnalysis tree from a root TokenUnit.
/// Replaces the legacy precursor-node logic with a cleaner, single-pass recursive build.
/// </summary>
public static class SpanBuilder
{
    public static SpanRoot Create(TokenUnit root, string fullText, string cardName, int clauseIndex)
    {
        var prefix = $"{cardName.Replace(' ', '_')}-line[{clauseIndex}]-index[{root.Match.RootMatch.Index}]-";
        var ctx = new SpanContext(fullText, prefix);

        var children = root.PropertyCaptures.Select(p => BuildNode(p, ctx)).ToList();

        return new SpanRoot
        {
            Name = root.Type.Name.ToFriendlyCase(TitleDisplayOption.Title),
            CapturePath = new(prefix + root.Match.CapturePath),
            CaptureTextOriginal = fullText.Substring(root.Match.RootMatch.Index, root.Match.RootMatch.Length),
            Start = root.Match.RootMatch.Index,
            End = root.Match.RootMatch.End,
            Length = root.Match.RootMatch.Length,
            ElementType = root is DefaultUnmatchedString ? TokenAnalysisElementType.UnmatchedTokenUnitRoot : TokenAnalysisElementType.TokenUnitRoot,
            Children = children,
            IsCollapsed = false,
            OriginalFullText = fullText,
            RootToken = root,
            CardName = cardName,
            ClauseIndex = clauseIndex
        };
    }

    static SpanNode BuildNode(PropertyCapture prop, SpanContext ctx)
    {
        return prop.Value switch
        {
            TokenUnitOneOf val => BuildTokenUnitOneOfBranch(val, prop, ctx),
            TokenUnitDistilled val => BuildDistilledBranch(val, prop, ctx),
            TokenUnit val => BuildTokenUnitBranch(val, prop, ctx),
            ManyOf val => BuildManyOfBranch(val, prop, ctx),
            CompoundOf val => BuildCompoundOfBranch(val, prop, ctx),
            OneOf val => BuildOneOfBranch(val, prop, ctx),
            OptionalOf val => BuildOptionalOfBranch(val, prop, ctx),
            DynamicOf val => BuildDynamicBranch(val, prop, ctx),
            PlaceholderCapture val => BuildLeaf(prop, ctx, val.Text, "placeholder", TokenAnalysisElementType.PlaceholderLeaf),
            bool val => BuildLeaf(prop, ctx, val.ToString().ToLower(), "bool", TokenAnalysisElementType.BoolLeaf),

            _ when prop.TemplatePropInfo.TemplatePropType == TemplatePropType.Enum
                => BuildLeaf(prop, ctx, prop.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.EnumLeaf),

            _ => throw new InvalidOperationException($"Unsupported: {prop.Value?.GetType().Name}")
        };
    }

    static SpanNode BuildTokenUnitBranch(TokenUnit tokenUnit, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.TemplatePropInfo.Name);
        var children = tokenUnit.PropertyCaptures.Select(p => BuildNode(p, ctx.ClearNameChain())).ToList();
        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitBranch, children, ctx);
    }

    static SpanNode BuildTokenUnitOneOfBranch(TokenUnitOneOf tokenUnitOneOf, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.TemplatePropInfo.Name);
        var inner = tokenUnitOneOf.PropertyCaptures.Single();
        var childNode = BuildNode(inner, ctx.ClearNameChain());
        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.OneOfItemBranch, new List<SpanNode> { childNode }, ctx);
    }

    static SpanNode BuildDynamicBranch(DynamicOf dynamicCapture, PropertyCapture prop, SpanContext ctx)
    {
        // 1. Prepare context for child: Clear inherited prefixes, but push the Type as a Suffix
        // This ensures when the child calls FormatName("Action"), it gets "Action: Specific Action"
        var typeName = dynamicCapture.Item.Value.GetType().Name;
        var childCtx = ctx.ClearNameChain().PushSuffix(typeName);

        SpanNode innerNode = dynamicCapture.Item.Value switch
        {
            TokenUnitOneOf val => BuildTokenUnitOneOfBranch(val, prop, childCtx),
            TokenUnit val => BuildTokenUnitBranch(val, prop, childCtx),
            _ => BuildLeaf(prop, childCtx, dynamicCapture.Item.Value.ToString()!, "enum", TokenAnalysisElementType.DynamicCaptureItemLeaf)
        };

        // 2. Return the branch. The branch name itself is just the prop name
        return CreateBranch(
            prop.Capture, 
            prop.TemplatePropInfo.Name, 
            prop.CaptureGroupPropPath,
            TokenAnalysisElementType.DynamicCaptureItemBranch,
            new List<SpanNode> { innerNode }, 
            ctx,
            neverCollapse: false);
    }

    static SpanNode BuildManyOfBranch(ManyOf manyOf, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.TemplatePropInfo.Name);
        var childCtx = ctx.ClearNameChain();
        var children = new List<SpanNode>();

        for (int i = 0; i < manyOf.Items.Count; i++)
        {
            var item = manyOf.Items[i];
            var itemPath = new CaptureGroupPropPath(prop.CaptureGroupPropPath + $"[{i}]");
            var itemLabel = $"#{i + 1}";

            if (manyOf.ManyItemVariant == CaptureTypeVariant.TokenUnit && item.Value is TokenUnit tu)
            {
                var itemCtx = childCtx.PushName(itemLabel);
                var innerTU = BuildTokenUnitBranch(tu, item.Capture, itemPath, itemCtx);
                children.Add(CreateBranch(item.Capture, itemLabel, itemPath, TokenAnalysisElementType.ManyOfItemBranch, new List<SpanNode> { innerTU }, ctx));
            }
            else
            {
                children.Add(BuildLeaf(item.Capture, itemLabel, itemPath, ctx, item.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.ManyOfItemLeaf));
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

    static SpanNode BuildCompoundOfBranch(CompoundOf compoundOf, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.TemplatePropInfo.Name);
        var childCtx = ctx.ClearNameChain();
        var children = new List<SpanNode>();

        for (int i = 0; i < compoundOf.Items.Count; i++)
        {
            var item = compoundOf.Items[i];
            var itemPath = new CaptureGroupPropPath(prop.CaptureGroupPropPath + $"[{i}]");
            var itemLabel = $"#{i + 1}";

            if (compoundOf.CaptureTypeVariant == CaptureTypeVariant.TokenUnit && item.Value is TokenUnit tu)
            {
                var itemCtx = childCtx.PushName(itemLabel);
                var innerTU = BuildTokenUnitBranch(tu, item.Capture, itemPath, itemCtx);
                children.Add(CreateBranch(item.Capture, itemLabel, itemPath, TokenAnalysisElementType.CompoundOfItemBranch, new List<SpanNode> { innerTU }, ctx));
            }
            else
            {
                children.Add(BuildLeaf(item.Capture, itemLabel, itemPath, ctx, item.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.CompoundOfItemLeaf));
            }
        }

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.CompoundOfBranch, children, ctx);
    }

    static SpanNode BuildOneOfBranch(OneOf oneOf, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.TemplatePropInfo.Name);
        var childCtx = ctx.ClearNameChain();
        var children = new List<SpanNode>();
        var itemLabel = oneOf.Item.TemplatePropInfo.Name;
        var itemPath = prop.CaptureGroupPropPath.Append(itemLabel);

        if (oneOf.Item.Value is TokenUnit tokenUnit)
        {
            var itemCtx = childCtx.PushName(prop.TemplatePropInfo.Name);
            var innerTU = BuildTokenUnitBranch(tokenUnit, oneOf.Item.Capture, itemPath, itemCtx);
            children.Add(CreateBranch(oneOf.Item.Capture, itemLabel, itemPath, TokenAnalysisElementType.CompoundOfItemBranch, new List<SpanNode> { innerTU }, ctx));
        }
        else
        {
            children.Add(BuildLeaf(oneOf.Item.Capture, itemLabel, itemPath, ctx, oneOf.Item.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.OneOfItemLeaf));
        }

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.OneOfItemBranch, children, ctx);
    }

    static SpanNode BuildOptionalOfBranch(OptionalOf optionalOf, PropertyCapture prop, SpanContext ctx)
    {
        if (optionalOf.Item.Value is not TokenUnit tokenUnit)
            throw new Exception("OptionalOf ItemObject must be a TokenUnit type");

        var name = ctx.FormatName(prop.TemplatePropInfo.Name);
        var childCtx = ctx.ClearNameChain();
        var children = new List<SpanNode>();
        var itemLabel = optionalOf.Item.TemplatePropInfo.Name;
        var itemPath = prop.CaptureGroupPropPath.Append(itemLabel);
        var itemCtx = childCtx.PushName(prop.TemplatePropInfo.Name);
        var innerTU = BuildTokenUnitBranch(tokenUnit, optionalOf.Item.Capture, itemPath, itemCtx);
        children.Add(CreateBranch(optionalOf.Item.Capture, itemLabel, itemPath, TokenAnalysisElementType.CompoundOfItemBranch, new List<SpanNode> { innerTU }, ctx));

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.OptionalOfItemBranch, children, ctx);
    }

    static SpanNode BuildDistilledBranch(TokenUnitDistilled tokenUnitDistilled, PropertyCapture prop, SpanContext ctx)
    {
        var name = ctx.FormatName(prop.TemplatePropInfo.Name);
        var childCtx = ctx.ClearNameChain();

        var children = tokenUnitDistilled.PropertyCaptures
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
                placeholder.TemplatePropInfo.Name,
                placeholder.CaptureGroupPropPath,
                TokenAnalysisElementType.TokenUnitDistilledBranch,
                subLeaves,
                ctx);

            children.Add(childBranch);
        }

        return CreateBranch(prop.Capture, name, prop.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitDistilledBranch, children, ctx);
    }

    static SpanNode BuildTokenUnitBranch(TokenUnit tu, ExtractedCapture cap, CaptureGroupPropPath path, SpanContext ctx)
    {
        var name = ctx.FormatName(tu.Type.Name);
        var children = tu.PropertyCaptures.Select(p => BuildNode(p, ctx.ClearNameChain())).ToList();
        return CreateBranch(cap, name, path, TokenAnalysisElementType.TokenUnitBranch, children, ctx);
    }

    static SpanBranch CreateBranch(ExtractedCapture cap, string name, CaptureGroupPropPath path, TokenAnalysisElementType type, List<SpanNode> children, SpanContext ctx, bool neverCollapse = false)
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
            IsCollapsed = neverCollapse ? false : SpanBranch.CalculateIsCollapsed(children)
        };
    }

    static SpanLeaf BuildLeaf(PropertyCapture prop, SpanContext ctx, string val, string typeName, TokenAnalysisElementType type) =>
        BuildLeaf(prop.Capture, prop.TemplatePropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence), prop.CaptureGroupPropPath, ctx, val, typeName, type);

    static SpanLeaf BuildLeaf(ExtractedCapture cap, string name, CaptureGroupPropPath path, SpanContext ctx, string val, string typeName, TokenAnalysisElementType type)
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