namespace MTGPlexer.RegexGeneration.GraphNodes;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenUnitOneOfNode : TokenUnitNode
{
    public TokenUnitOneOfNode(RegexNode parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
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