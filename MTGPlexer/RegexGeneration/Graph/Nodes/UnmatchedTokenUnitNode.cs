namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class UnmatchedTokenUnitNode : TokenUnitNode
{
    public UnmatchedTokenUnitNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
    }
}
