namespace Glyphotype.RegexGeneration.Graph.Nodes;

public class GlyphCompoundOfSecondItemNode : NamedGroupNode
{
    public override CaptureNodeKind NodeKind => CaptureNodeKind.Internals;

    public GlyphCompoundOfSecondItemNode(RegexNode parentNode, Navigation navigation)
    : base(parentNode, navigation)
    {
    }


    protected override object GetValue(CaptureTrace captureTrace)
    {
        throw new NotImplementedException();
    }
}