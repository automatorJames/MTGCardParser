namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a bool property on a TokenUnit or x-Of. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public class BoolNode : LeafNode
{
    public BoolNode(RegexNode parentNode, PropertySnippet propertySnippet) 
        : base(parentNode, propertySnippet)
    {
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        builder.OpenNamedGroup(this);
        builder.AddAlternateValues(ScalarAlternateSet.Alternates);
        builder.CloseGroup(GroupQuantifier.Optional);
    }

    public override object GetValueSingle(Capture capture)
    {
        // Simply return "true", because TerminalNode already validated that the
        // named group exists, and therefore this bool check has already succeeded

        return true;
    }
}