namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record TokenUnitNode : BranchNode
{
    public TokenUnitNode(PropertySnippet propertySnippet) : base(propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        TemplatePropInfo templatePropInfo = new(PropertySnippet.Prop);
        builder.OpenGroup(templatePropInfo);
        ConcatenatingComposer.Instance.Compose(builder, ChildSegments.ToList());
        var groupIsOptional = PropertySnippet.Prop.IsDefined(typeof(OptionalComponentAttribute));
        GroupQuantifier? groupQuantifier = groupIsOptional ? GroupQuantifier.Optional : null;
        builder.CloseGroup(groupQuantifier);
    }
}
