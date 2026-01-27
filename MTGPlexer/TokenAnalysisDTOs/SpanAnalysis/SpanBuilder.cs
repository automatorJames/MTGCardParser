namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// Factory service that builds a SpanAnalysis tree from a root TokenUnit.
/// </summary>
public static class SpanBuilder
{
    public static SpanRoot Create(TokenUnit root, string text, string card, int line)
    {
        var prefix = $"{card.Replace(' ', '_')}-line[{line}]-index[{root.Match.RootMatch.Index}]-";
        var ctx = new SpanContext(text, prefix);

        return new SpanRoot
        {
            Name = root.Type.Name.ToFriendlyCase(TitleDisplayOption.Title),
            CapturePath = new(prefix + root.Match.CapturePath),
            CaptureTextOriginal = text.Substring(root.Match.RootMatch.Index, root.Match.RootMatch.Length),
            Start = root.Match.RootMatch.Index,
            End = root.Match.RootMatch.End,
            Length = root.Match.RootMatch.Length,
            ElementType = root is DefaultUnmatchedString
                ? TokenAnalysisElementType.UnmatchedTokenUnitRoot
                : TokenAnalysisElementType.TokenUnitRoot,
            Children = root.PropertyCaptures.Select(p => BuildNode(p, ctx)).ToList(),
            RootToken = root,
            CardName = card,
            ClauseIndex = line,
            OriginalFullText = text
        };
    }

    private static SpanNode BuildNode(PropertyCapture prop, SpanContext ctx)
    {
        return prop.TemplatePropInfo.TemplatePropType switch
        {
            TemplatePropType.TokenUnit =>
                BuildTokenUnitBranch((TokenUnit)prop.Value, prop.Capture, prop.CaptureGroupPropPath, ctx.PushName(prop.TemplatePropInfo.Name)),

            TemplatePropType.TokenUnitOneOf =>
                BuildTokenUnitOneOfBranch((TokenUnitOneOf)prop.Value, prop, ctx),

            TemplatePropType.Dynamic =>
                BuildDynamicBranch((DynamicOf)prop.Value, prop, ctx),

            TemplatePropType.DistilledValue =>
                BuildDistilledBranch((TokenUnitDistilled)prop.Value, prop, ctx),

            TemplatePropType.ManyOf or TemplatePropType.CompoundOf or TemplatePropType.OneOf or TemplatePropType.OptionalOf =>
                BuildXOfBranch((XOf)prop.Value, prop, ctx),

            TemplatePropType.Placeholder =>
                CreateLeaf(prop, ctx, ((PlaceholderCapture)prop.Value).Text, "placeholder", TokenAnalysisElementType.PlaceholderLeaf),

            TemplatePropType.Bool =>
                CreateLeaf(prop, ctx, prop.Value.ToString()!.ToLower(), "bool", TokenAnalysisElementType.BoolLeaf),

            _ =>
                CreateLeaf(prop, ctx, prop.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", TokenAnalysisElementType.EnumLeaf)
        };
    }

    private static SpanBranch BuildTokenUnitBranch(TokenUnit tu, ExtractedCapture cap, CaptureGroupPropPath path, SpanContext ctx)
    {
        var name = ctx.FormatName(tu.Type.Name);

        // Children of visible branches start with a fresh context.
        var children = tu.PropertyCaptures.Select(p => BuildNode(p, ctx.Clear())).ToList();

        return CreateBranch(cap, name, path, TokenAnalysisElementType.TokenUnitBranch, children, ctx);
    }

    private static SpanBranch BuildTokenUnitOneOfBranch(TokenUnitOneOf tuOneOf, PropertyCapture prop, SpanContext ctx)
    {
        var childCtx = ctx.PushName(prop.TemplatePropInfo.Name);
        var innerProp = tuOneOf.PropertyCaptures.Single();
        var children = new List<SpanNode> { BuildNode(innerProp, childCtx) };

        return CreateBranch(prop.Capture, "one-of-wrapper", prop.CaptureGroupPropPath, TokenAnalysisElementType.OneOfItemBranch, children, ctx)
            with { IsCollapsed = true };
    }

    private static SpanBranch BuildDynamicBranch(DynamicOf dyn, PropertyCapture prop, SpanContext ctx)
    {
        var typeName = dyn.Item.Value.GetType().Name;
        var childCtx = ctx.PushName(prop.TemplatePropInfo.Name);
        var path = prop.CaptureGroupPropPath.Append(typeName);

        SpanNode inner = dyn.Item.Value switch
        {
            TokenUnit tu => BuildTokenUnitBranch(tu, prop.Capture, path, childCtx),
            _ => CreateLeaf(prop.Capture, typeName, path, childCtx, dyn.Item.Value.ToString()!, "enum", TokenAnalysisElementType.DynamicCaptureItemLeaf)
        };

        return CreateBranch(prop.Capture, prop.TemplatePropInfo.Name, prop.CaptureGroupPropPath, TokenAnalysisElementType.DynamicCaptureItemBranch, [inner], ctx);
    }

    private static SpanBranch BuildXOfBranch(XOf xOf, PropertyCapture prop, SpanContext ctx)
    {
        var children = new List<SpanNode>();
        var childCtx = (xOf is OneOf or OptionalOf) ? ctx.PushName(prop.TemplatePropInfo.Name) : ctx.Clear();

        var items = xOf switch
        {
            ManyOf many => many.Items,
            CompoundOf comp => comp.Items,
            OneOf one => [one.Item],
            OptionalOf opt => [opt.Item],
            _ => new List<PolyItemCapture>()
        };

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var identifier = item.DistinguishingName ?? (xOf is OneOf or OptionalOf ? item.Type.Name : $"item[{i}]");
            var path = prop.CaptureGroupPropPath.Append(identifier);
            var label = (xOf is OneOf or OptionalOf) ? prop.TemplatePropInfo.Name : $"#{i + 1}";

            if (item.Value is TokenUnit tu)
            {
                children.Add(BuildTokenUnitBranch(tu, item.Capture, path, childCtx));
            }
            else
            {
                children.Add(CreateLeaf(item.Capture, label, path, ctx, item.Value.ToString()!.ToFriendlyCase(TitleDisplayOption.Lower), "enum", MapXOfElementType(xOf, false)));
            }
        }

        // Variable name changed to 'manyWithConj' to avoid scope conflict
        if (xOf is ManyOf { Conjunction: not null } manyWithConj)
        {
            var conjPath = prop.CaptureGroupPropPath.Append("Conjunction");
            children.Add(CreateLeaf(manyWithConj.ConjunctionCapture, "Conjunction", conjPath, ctx, manyWithConj.Conjunction.ToString()!.ToLower(), "enum", TokenAnalysisElementType.ConjunctionLeaf));
        }

        var branch = CreateBranch(prop.Capture, ctx.FormatName(prop.TemplatePropInfo.Name), prop.CaptureGroupPropPath, MapXOfContainerType(xOf), children, ctx);

        if (xOf is OneOf or OptionalOf)
        {
            return branch with { IsCollapsed = true };
        }

        return branch;
    }

    private static SpanBranch BuildDistilledBranch(TokenUnitDistilled dist, PropertyCapture prop, SpanContext ctx)
    {
        var children = dist.PropertyCaptures
            .Where(p => !dist.DistilledVals.ContainsKey(p))
            .Select(p => BuildNode(p, ctx.Clear()))
            .ToList();

        foreach (var (ph, vals) in dist.DistilledVals)
        {
            var subs = vals.Select(v => new SpanSubLeaf
            {
                Name = v.Key.Name,
                TerminalValString = v.Value.ToString()!.ToLower(),
                TerminalType = "distilled",
                ElementType = TokenAnalysisElementType.DistilledValueSubLeaf,
                Start = ph.Capture.Index,
                End = ph.Capture.End,
                Length = ph.Capture.Length,
                CaptureTextOriginal = ph.Capture.Value,
                CapturePath = new(ctx.PathPrefix + ph.CaptureGroupPropPath.Append(v.Key.Name))
            }).Cast<SpanNode>().ToList();

            children.Add(CreateBranch(ph.Capture, ph.TemplatePropInfo.Name, ph.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitDistilledBranch, subs, ctx));
        }

        return CreateBranch(prop.Capture, ctx.FormatName(prop.TemplatePropInfo.Name), prop.CaptureGroupPropPath, TokenAnalysisElementType.TokenUnitDistilledBranch, children, ctx);
    }

    #region Helpers

    private static SpanBranch CreateBranch(ExtractedCapture c, string n, CaptureGroupPropPath p, TokenAnalysisElementType t, List<SpanNode> ch, SpanContext ctx)
    {
        return new SpanBranch
        {
            Name = n,
            CapturePath = new(ctx.PathPrefix + p),
            Start = c.Index,
            Length = c.Length,
            End = c.End,
            CaptureTextOriginal = ctx.FullText.Substring(c.Index, c.Length),
            ElementType = t,
            Children = ch,
            IsCollapsed = SpanBranch.CalculateIsCollapsed(ch)
        };
    }

    private static SpanLeaf CreateLeaf(PropertyCapture p, SpanContext ctx, string v, string tn, TokenAnalysisElementType t)
    {
        return CreateLeaf(p.Capture, p.TemplatePropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence), p.CaptureGroupPropPath, ctx, v, tn, t);
    }

    private static SpanLeaf CreateLeaf(ExtractedCapture c, string n, CaptureGroupPropPath p, SpanContext ctx, string v, string tn, TokenAnalysisElementType t)
    {
        return new SpanLeaf
        {
            Name = n,
            CapturePath = new(ctx.PathPrefix + p),
            Start = c.Index,
            Length = c.Length,
            End = c.End,
            CaptureTextOriginal = ctx.FullText.Substring(c.Index, c.Length),
            ElementType = t,
            TerminalValString = v,
            TerminalType = tn
        };
    }

    private static TokenAnalysisElementType MapXOfContainerType(XOf x)
    {
        return x switch
        {
            ManyOf => TokenAnalysisElementType.ManyOfBranch,
            CompoundOf => TokenAnalysisElementType.CompoundOfBranch,
            _ => TokenAnalysisElementType.OneOfItemBranch
        };
    }

    private static TokenAnalysisElementType MapXOfElementType(XOf x, bool isBranch)
    {
        return x switch
        {
            ManyOf => isBranch ? TokenAnalysisElementType.ManyOfItemBranch : TokenAnalysisElementType.ManyOfItemLeaf,
            CompoundOf => isBranch ? TokenAnalysisElementType.CompoundOfItemBranch : TokenAnalysisElementType.CompoundOfItemLeaf,
            _ => isBranch ? TokenAnalysisElementType.CompoundOfItemBranch : TokenAnalysisElementType.OneOfItemLeaf
        };
    }

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