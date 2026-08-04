namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>A <see cref="RegexNode"/> that was reached via reflection over a type/property, and so carries that reflection metadata as <see cref="Navigation"/>.</summary>
public abstract class GroupNode : RegexNode
{
    /// <summary>The reflection metadata (type, property, declared patterns, quantifier, etc.) this node was built from.</summary>
    public Navigation Navigation { get; }

    /// <summary>The quantifier applied to this group's closing brick, if any.</summary>
    public virtual Quantifier? Quantifier => Navigation.Quantifier;

    protected GroupNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation.Name)
    {
        Navigation = navigation;
    }
}