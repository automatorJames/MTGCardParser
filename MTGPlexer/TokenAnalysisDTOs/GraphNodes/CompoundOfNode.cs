using MTGPlexer.TokenAnalysisDTOs.GraphNodes;

namespace MTGPlexer.RegexGeneration.RegexSegments;

public record CompoundOfNode : WrapperPropertyNode
{
    public List<WrappedNode> WrappedNodes { get; } = [];

    public CompoundOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(PropertySnippet.ToTemplatePropInfo());
        builder.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        TemplateNodeForComposition.ComposeRegexLines(builder);
        builder.AddTextLine(" ?");
        builder.CloseGroup(GroupQuantifier.OneOrMore);
        builder.AddNegativeSpaceLookbehindBoundary();
        builder.CloseGroup();
    }

    protected override object GetPropertyValue(Capture capture)
    {
        //List<PolyItemCapture> hydratedItems = [];
        //
        //if (capture is not Group group)
        //    throw new Exception();
        //
        //for (int i = 0; i < group.Captures.Count; i++)
        //{
        //    var ordinalCapture = group.Captures[i];
        //    MatchTraversalState state = new(_genericType, parentTokenUnitMatch, TemplatePropInfo.Prop.Name);
        //    var childItem = _regexProp.GetPropertyValue(state, ordinalCapture, out var ordinalResult);
        //    PolyItemCapture hydratedItem = new(childItem, ordinalCapture, TemplatePropInfo);
        //    hydratedItems.Add(hydratedItem);
        //}
        //
        //var compoundType = typeof(CompoundOf<>).MakeGenericType(GenericType);
        //var compoundPropVal = Activator.CreateInstance(compoundType, hydratedItems);
        //
        //result = ValueResult.Success;
        //return compoundPropVal;

        throw new NotImplementedException();
    }


    public override string ToString() => base.ToString();
}