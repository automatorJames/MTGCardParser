namespace MTGPlexer.RegexGeneration.GraphNodes;

public record OptionalOfNode : WrapperPropertyNode
{
    public OptionalOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        if (!GenericType.IsAssignableTo(typeof(TokenUnit)))
            throw new Exception($"{nameof(OptionalOfNode)} expects '{nameof(TokenUnit)}' type, but found '{GenericType.Name}' type");
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this);
        TemplateNodeForComposition.ComposeRegexLines(builder);
        GroupQuantifier? groupQuantifier = GroupQuantifier.Optional;
        builder.CloseGroup(groupQuantifier);
    }

    public override object GetValue(Capture capture)
    {
        //var itemCapture = parentTokenUnitMatch[LeafName + "_" + TemplatePropInfo.Prop.Name].Single();
        //MatchTraversalState typeMatch = new(GenericType, parentTokenUnitMatch, TemplatePropInfo.Prop.Name);
        //var tokenUnitChild = TokenUnit.InstantiateFromMatch(typeMatch, out var localResult);
        //PolyItemCapture hydratedItem = new(tokenUnitChild, itemCapture, TemplatePropInfo);
        //var optionalType = typeof(OptionalOf<>).MakeGenericType(GenericType);
        //var optionalPropVal = Activator.CreateInstance(optionalType, hydratedItem);
        //
        //result = ValueResult.Success;
        //return optionalPropVal;

        throw new NotImplementedException();
    }

    public override string ToString() => base.ToString();
}