namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a bool property on a TokenUnit or x-Of. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public class BoolNode : ScalarContainerNode
{
    protected override GroupQuantifier? Quantifier => GroupQuantifier.Optional;
    protected override bool OneOrMoreRegexPatternsRequired => true;

    public BoolNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
    }

    public override object GetValueSingle(Capture capture)
    {
        // Simply return "true", because TerminalNode already validated that the
        // named group exists, and therefore this bool check has already succeeded

        return true;
    }
}