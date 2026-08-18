namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a bool property on a TokenUnit or x-Of. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public class BoolNode : NamedGroupNode
{
    public override CaptureNodeKind NodeKind => CaptureNodeKind.Bool;
    public override Quantifier? Quantifier => MTGPlexer.Quantifier.Optional;
    protected override bool OneOrMoreRegexPatternsRequired => true;

    public BoolNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
    }

    /// <summary>Always true: a successful capture of this optional group is itself proof the pattern matched.</summary>
    protected override object GetValue(CaptureTrace captureTrace)
    {
        if (captureTrace.Count != 1)
            throw new Exception($"{nameof(BoolNode)} expects exactly one capture");

        return true;
    }
}