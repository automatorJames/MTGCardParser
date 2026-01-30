namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record OneOfNode : WrapperPropertyNode
{
    public OneOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this, spaceDisposition: SpaceDisposition.DisallowedLocal);
        AlternatingComposer.Instance.Compose(builder, TemplateNodesForComposition.Cast<Node>().ToList());
        builder.CloseGroup();
    }

    public override object GetValue(Capture capture)
    {
        //PolyItemCapture foundPolyMatchValue = null;
        //int foundPropIndex = 0;
        //
        //for (int i = 0; i < _regexProps.Count; i++)
        //{
        //    var regexProp = _regexProps[i];
        //    var oneOfItemVariantCapture = parentTokenUnitMatch[LeafName + "_" + regexProp.LeafName].SingleOrDefault();
        //
        //    if (oneOfItemVariantCapture == null)
        //        continue;
        //
        //    MatchTraversalState state = new(GenericType, parentTokenUnitMatch, regexProp.LeafName);
        //    var childItem = regexProp.GetPropertyValue(state, oneOfItemVariantCapture, out var ordinalResult);
        //    foundPolyMatchValue = new(childItem, oneOfItemVariantCapture, TemplatePropInfo);
        //
        //    goto ItemHasBeenFound;
        //}
        //
        //// If none of the regex props found a match, there's a problem
        //throw new Exception($"Failed to extract value for OneOfProp from capture '{scopedCapture.Value}'");
        //
        //ItemHasBeenFound:;
        //
        //var genericTypeDefinition = _regexProps.Count switch
        //{
        //    2 => typeof(OneOf<,>),
        //    3 => typeof(OneOf<,,>),
        //    _ => throw new Exception($"One-of regex prop count of {_regexProps.Count} not supported")
        //};
        //
        //var oneOfCaptureType = genericTypeDefinition.MakeGenericType(_regexProps.Select(x => x.TemplatePropInfo.UnderlyingType).ToArray());
        //var oneOfPropVal = Activator.CreateInstance(oneOfCaptureType, foundPolyMatchValue, foundPropIndex);
        //
        //result = ValueResult.Success;
        //return oneOfPropVal;

        throw new NotImplementedException();
    }

    public override string ToString() => base.ToString();
}