namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class IntNode : NamedGroupNode
{
    protected override bool OneOrMoreRegexPatternsRequired => true;
    public IntNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
    }

    protected override void AddReflectedChildren(List<RegexNode> children) =>
        children.AddRange(
            Navigation.Patterns.Select((x, idx) => new ScalarNode(
                    parentNode: this,
                    name: $"Countable-Pattern" + (idx > 0 ? $"-{idx}" : ""),
                    scalarValue: true,
                    regex: x,
                    positionAmongSiblings: idx
                )));

    protected override object GetValue(CaptureInfo captureInfo)
    {
        // Check if the capture itself is a singular int
        if (captureInfo.Count == 1 && int.TryParse(captureInfo.CaptureValue, out int parsedInt))
            return parsedInt;

        // Otherwise, return the count of occurrences of the match
        return captureInfo.Count;
    }
}