namespace MTGPlexer.RegexGeneration.GraphNodes;

public class TokenUnitNode : CaptureNode
{
    public TokenUnitNode(Node parentNode, INavigable navigation) : base(parentNode, navigation)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this);
        ConcatenatingComposer.Instance.Compose(builder, Children.ToList());
        GroupQuantifier? groupQuantifier = IsOptional ? GroupQuantifier.Optional : null;
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
