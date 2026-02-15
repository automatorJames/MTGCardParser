namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class IntNode : ScalarContainerNode
{
    protected override bool OneOrMoreRegexPatternsRequired => true;
    public IntNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
    }

    public override object GetValueSingle(Capture capture)
    {
        return Children
            .OfType<INamedScalarValue>()
            .FirstOrDefault(x => x.Name == capture.Value)
            .ScalarValue
            ?? throw new Exception($"Found no matching values for enum '{Navigation.UnderlyingType.Name}' from match string '{capture.Value}'");
    }

    //public override object GetValueSingle(Capture capture)
    //{
    //    // Simply return "true", because TerminalNode already validated that the
    //    // named group exists, and therefore this bool check has already succeeded
    //
    //    return true;
    //}
}