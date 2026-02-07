namespace MTGPlexer.RegexGeneration.GraphNodes;

public class IntNode : LeafNode
{

    public IntNode(RegexNode parentNode, PropertySnippet propertySnippet) 
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