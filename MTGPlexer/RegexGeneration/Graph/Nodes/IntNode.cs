namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class IntNode : ScalarContainerNode
{

    public IntNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
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