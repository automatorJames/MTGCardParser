

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

    public NamedGroupNode AddWrappedNamedGroupChild(PropNavigation wrapperPropNavigation, Type typeToWrap, string groupNameAppendix)
    {
        NamedGroupNode wrappedNamedGroupChild = GetNamedGroupChild(this, wrapperPropNavigation, typeToWrap, groupNameAppendix);
        Children.Add(wrappedNamedGroupChild);
        return wrappedNamedGroupChild;
    }

    public void AddWrappedBrick(string brickName, RegexBrick brick) =>
        Children.Add(new TransparentBrickWrapperNode(this, brickName, brick));

    public void AddNode(RegexNode node) =>
        Children.Add(node);

    public override void AppendRegexBricks(RegexCollector collector)
    {
        base.AppendRegexBricks(collector);
    }
}
