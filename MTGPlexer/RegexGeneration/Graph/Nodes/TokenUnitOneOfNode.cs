namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenUnitOneOfNode : TokenUnitNode
{
    protected override Joiner Joiner => Joiner.Pipe;
    public override CaptureNodeType NodeType => CaptureNodeType.OneOf;
    public TokenUnitOneOfNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
    }
}