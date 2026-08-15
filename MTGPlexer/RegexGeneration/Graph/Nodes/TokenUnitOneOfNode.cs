namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class TokenUnitOneOfNode : TokenUnitNode
{
    protected override Joiner Joiner => Joiner.Pipe;
    public override CaptureNodeKind NodeType => CaptureNodeKind.OneOf;
    public TokenUnitOneOfNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
    }
}