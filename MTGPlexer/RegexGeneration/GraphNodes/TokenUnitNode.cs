namespace MTGPlexer.RegexGeneration.GraphNodes;

public record TokenUnitNode : CaptureNode
{
    public TokenUnitNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this);
        ConcatenatingComposer.Instance.Compose(builder, Children.ToList());
        var groupIsOptional = PropertySnippet.Prop.IsDefined(typeof(OptionalComponentAttribute));
        GroupQuantifier? groupQuantifier = groupIsOptional ? GroupQuantifier.Optional : null;
        builder.CloseGroup(groupQuantifier);
    }

    public override object GetValue(Capture capture)
    {
        //var rematch = 
        //MatchTraversalState typeMatch = new(TemplatePropInfo.UnderlyingType, parentTokenUnitMatch, TemplatePropInfo.Name, scopedCapture: scopedCapture);
        //var tokenUnitInstance = TokenUnit.InstantiateFromMatch(typeMatch, out result);
        //
        //return tokenUnitInstance;

        throw new NotImplementedException();
    }
}
