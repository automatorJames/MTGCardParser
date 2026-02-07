namespace MTGPlexer.RegexGeneration.GraphNodes;

public class TokenUnitCompoundNode : TokenUnitNode
{
    public TokenUnitCompoundNode(RegexNode parentNode, INavigable navigable) : base(parentNode, navigable)
    {
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        builder.OpenNamedGroup(this);
        AlternatingComposer.Instance.Compose(builder, Children);
        builder.CloseGroup();
    }

    public override string ToString() => base.ToString();
}