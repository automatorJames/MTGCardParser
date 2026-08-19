namespace Glyphotype.RegexGeneration.Graph.Nodes;

public class GlyphOneOfNode : GlyphNode
{
    public override CaptureNodeKind NodeKind => CaptureNodeKind.OneOf;
    public GlyphOneOfNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
    }

    public override bool TryHydrate(CaptureTrace captureTrace, out Glyph glyph)
    {
        glyph = null;
        var instance = (Glyph)Activator.CreateInstance(Navigation.NodeType);

        // Counter to track successfully set children
        int childrenSuccessfullySet = 0;

        foreach (var child in NamedGroupChildren)
        {
            var setResult = child.SetPropertyValue(instance, captureTrace.CaptureContext);

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