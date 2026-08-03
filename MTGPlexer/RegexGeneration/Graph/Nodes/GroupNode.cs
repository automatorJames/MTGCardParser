namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class GroupNode : RegexNode
{
    public Navigation Navigation { get; }
    protected virtual Quantifier? Quantifier => Navigation.Quantifier;

    protected GroupNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation.Name)
    {
        Navigation = navigation;
    }
}