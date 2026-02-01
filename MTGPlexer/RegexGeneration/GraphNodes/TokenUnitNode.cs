namespace MTGPlexer.RegexGeneration.GraphNodes;

public class TokenUnitNode : BranchNode
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

    public override object TryGetValue(CaptureDictionary captureDictionary, out CaptureValueResult result)
    {
        var instance = (TokenUnit)Activator.CreateInstance(UnderlyingType);

        foreach (var captureNode in CaptureNodes)
            captureNode.SetPropertyValue(captureDictionary, instance);

        // todo: how should the actual value be determined?
        result = CaptureValueResult.FoundWithValue;

        return instance;
    }
}
