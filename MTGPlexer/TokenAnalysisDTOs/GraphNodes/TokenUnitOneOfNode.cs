namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenUnitOneOfNode : TokenUnitNode
{
    public TokenUnitOneOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        // If there are no text sgements to render, the OneOf container itself doesn't need spaces between its alternating members
        SpaceDisposition? spaceDisposition = !Children.Any(x => x is TextNode) ? SpaceDisposition.DisallowedLocal : null;

        builder.OpenGroup(PropertySnippet.ToTemplatePropInfo(), spaceDisposition: spaceDisposition);
        AlternatingComposer.Instance.Compose(builder, Children);
        builder.CloseGroup();
    }

    public override string ToString() => base.ToString();
}