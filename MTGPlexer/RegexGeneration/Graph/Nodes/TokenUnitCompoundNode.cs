namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class TokenUnitCompoundNode : TokenUnitNode
{
    protected override Joiner Joiner { get; }

    public TokenUnitCompoundNode(RegexNode parentNode, TypeNavigation navigation) : base(parentNode, navigation)
    {
        Joiner = navigation.UnderlyingType.GetCustomAttribute<CompoundJoinerAttribute>()?.Joiner 
            ?? Joiner.Space;
    }
}