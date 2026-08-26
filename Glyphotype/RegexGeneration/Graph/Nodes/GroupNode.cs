namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>A <see cref="RegexNode"/> that was reached via reflection over a type/property, and so carries that reflection metadata as <see cref="Navigation"/>.</summary>
public abstract class GroupNode : RegexNode
{
    /// <summary>The reflection metadata (type, property, declared patterns, quantifier, etc.) this node was built from.</summary>
    public Navigation Navigation { get; }

    /// <summary>The quantifier applied to this group's closing brick, if any.</summary>
    public virtual Quantifier? Quantifier => Navigation.Quantifier;

    /// <summary>A group is nullable exactly when its own quantifier permits zero occurrences - regardless of whether that quantifier came from <see cref="Navigation"/> or, like <see cref="BoolNode"/>'s, is hardcoded by the node type itself.</summary>
    public override bool IsNullable => Quantifier is Glyphotype.Quantifier.Optional or Glyphotype.Quantifier.AnyNumber;

    protected GroupNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation.Name)
    {
        Navigation = navigation;
    }
}