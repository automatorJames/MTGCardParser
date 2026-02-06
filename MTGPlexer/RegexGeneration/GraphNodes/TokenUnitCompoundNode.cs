namespace MTGPlexer.RegexGeneration.GraphNodes;

public class TokenUnitCompoundNode : TokenUnitNode
{
    public TokenUnitCompoundNode(RegexNode parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this);
        AlternatingComposer.Instance.Compose(builder, Children);
        builder.CloseGroup();
    }

    public override string ToString() => base.ToString();
}