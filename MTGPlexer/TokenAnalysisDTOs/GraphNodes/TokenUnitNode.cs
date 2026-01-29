
namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record TokenUnitNode : BranchNode
{
    public TokenUnitNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        TemplatePropInfo templatePropInfo = new(PropertySnippet.Prop);
        builder.OpenGroup(templatePropInfo);
        ConcatenatingComposer.Instance.Compose(builder, Children.ToList());
        var groupIsOptional = PropertySnippet.Prop.IsDefined(typeof(OptionalComponentAttribute));
        GroupQuantifier? groupQuantifier = groupIsOptional ? GroupQuantifier.Optional : null;
        builder.CloseGroup(groupQuantifier);
    }

    protected override object GetPropertyValue(Capture capture)
    {
        throw new NotImplementedException();
    }
}
