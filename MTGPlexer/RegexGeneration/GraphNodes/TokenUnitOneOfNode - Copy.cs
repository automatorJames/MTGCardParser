namespace MTGPlexer.RegexGeneration.GraphNodes;

public class TokenUnitCompoundNode : TokenUnitNode
{
    public TokenUnitCompoundNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        // If there are no text sgements to render, the OneOf container itself doesn't need spaces between its alternating members
        SpaceDisposition? spaceDisposition = !Children.Any(x => x is TextNode) ? SpaceDisposition.DisallowedLocal : null;

        builder.OpenNamedGroup(this, spaceDisposition: spaceDisposition);
        AlternatingComposer.Instance.Compose(builder, Children);
        builder.CloseGroup();
    }

    public override string ToString() => base.ToString();
}