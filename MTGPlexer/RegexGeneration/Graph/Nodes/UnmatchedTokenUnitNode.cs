namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>Represents <see cref="UnmatchedString"/>, the fallback token type for source text that matched no other <see cref="TokenUnit"/>.</summary>
public class UnmatchedTokenUnitNode : TokenUnitNode
{
    public UnmatchedTokenUnitNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
    }
}
