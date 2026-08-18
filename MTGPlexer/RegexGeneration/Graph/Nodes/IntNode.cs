namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class IntNode : NamedGroupNode
{
    protected override bool OneOrMoreRegexPatternsRequired => true;

    public override CaptureNodeKind NodeKind => CaptureNodeKind.Int;

    public IntNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
    }

    /// <summary>The captured text parsed as an int, or (if it doesn't parse, e.g. a repeated marker like "+1/+1" counters) the number of times it occurred.</summary>
    protected override object GetValue(CaptureTrace captureTrace)
    {
        if (captureTrace.Count == 1 && int.TryParse(captureTrace.CaptureValue, out int parsedInt))
            return parsedInt;

        return captureTrace.Count;
    }
}