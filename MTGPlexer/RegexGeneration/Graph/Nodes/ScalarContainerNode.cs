

namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class ScalarContainerNode : NamedGroupNode
{
    protected ScalarContainerNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    protected override void AddReflectedChildren(List<RegexNode> children) =>
        children.AddRange(
            Navigation.Patterns.Select((x, idx) => new ScalarNode(
                    parentNode: this,
                    name: $"{GetType().Name}-Pattern" + (idx > 0 ? $"-{idx}" : ""),
                    scalarValue: true,
                    regex: x,
                    positionAmongSiblings: idx
                )));

    protected override object GetValue(CaptureContext captureContext)
    {
        if (captureContext.Count != 1)
            throw new Exception($"{nameof(ScalarContainerNode)} expects exactly one capture");
    
        return GetValueSingle(captureContext.Capture);
    }

    public abstract object GetValueSingle(Capture capture);
}
