namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// A <see cref="OneOfBase"/>: its properties are alternatives, exactly one of which matches.
/// <para>
/// Its <see cref="Glyph.Nibs"/> may also put literal text around the alternatives - one contiguous run of
/// properties with text before and/or after it, as <see cref="OneOfBase.ValidateStructure"/> enforces - e.g.
/// <c>[@"\{", Prop(Colorless), Prop(Symbol), @"\}"]</c> for a braced symbol. That renders as
/// <c>\{(colorless|symbol)\}</c>: the pipe only ever sits between two alternatives, and nothing is inserted
/// between the text and the alternatives - any space wanted there belongs in the text itself (see
/// <see cref="JoinerRules.Between"/>). The alternatives are grouped so the text binds to all of them rather than
/// to the first and last alone - a plain group, which captures nothing under <see cref="RegexOptions.ExplicitCapture"/>.
/// A joiner the one-of carries inside its own group (as an optional one-of does - see
/// <see cref="NamedGroupNode.AppendOwnRegexBricks"/>) is grouped around in the same way, or it would precede the first
/// alternative alone.
/// </para>
/// </summary>
public class GlyphOneOfNode : GlyphNode
{
    public GlyphOneOfNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    protected override void AppendInnerContentBricks(RegexCollector collector)
    {
        var carriesJoiner = LeadingJoinerPlacement == JoinerPlacement.InsideNodeLeading || TrailingJoinerSuccessor is not null;

        if (!carriesJoiner && !Children.OfType<TextNode>().Any())
        {
            base.AppendInnerContentBricks(collector);
            return;
        }

        var firstAlternative = Children.FindIndex(x => x is NamedGroupNode);
        var lastAlternative = Children.FindLastIndex(x => x is NamedGroupNode);

        for (int i = 0; i < Children.Count; i++)
        {
            if (i == firstAlternative)
                collector.Append(new RegexBrick(this, "("));

            Children[i].AppendRegexBricks(collector);

            if (i == lastAlternative)
                collector.Append(new RegexBrick(this, ")"));
        }
    }

    public override bool TryHydrate(CaptureTrace captureTrace, out Glyph glyph)
    {
        glyph = null;
        var instance = (Glyph)Activator.CreateInstance(Navigation.NodeType);

        // Counter to track successfully set children
        int childrenSuccessfullySet = 0;

        foreach (var child in NamedGroupChildren)
        {
            var setResult = child.SetPropertyValue(instance, captureTrace);

            if (setResult)
                childrenSuccessfullySet++;
        }

        // In a one-of node, we expect exactly one child to match
        if (childrenSuccessfullySet != 1)
            return false;

        instance.CaptureContext = captureTrace.CaptureContext;
        glyph = instance;

        return true;
    }
}
