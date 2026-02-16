


namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class AnonymousGroupNode : GroupNode
{
    protected override Joiner Joiner => Joiner.None;
    protected override GroupQuantifier? Quantifier { get; }

    public AnonymousGroupNode(RegexNode parentNode, string name, GroupQuantifier? groupQuantifier = null) 
        : base(parentNode, name)
    {
        Quantifier = groupQuantifier;
    }

    public NamedGroupNode AddWrappedNamedGroupChild(TypeNavigation wrapperNavigation, Type typeToWrap, string groupName)
    {
        NamedGroupNode wrappedNamedGroupChild = GetNamedGroupChild(this, wrapperNavigation, typeToWrap, groupName);
        Children.Add(wrappedNamedGroupChild);
        return wrappedNamedGroupChild;
    }

    public void AddWrappedBrickContent(string nodeName, string brickRegex, string brickComment) =>
        Children.Add(new TransparentBrickWrapperNode(this, nodeName, new RegexBrick(this, brickRegex, brickComment)));

    public void AddNode(RegexNode node) =>
        Children.Add(node);
}
