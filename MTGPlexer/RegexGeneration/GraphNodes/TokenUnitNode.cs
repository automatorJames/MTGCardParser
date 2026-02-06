
namespace MTGPlexer.RegexGeneration.GraphNodes;

public class TokenUnitNode : BranchNode
{
    public TokenUnitNode(RegexNode parentNode, INavigable navigation) : base(parentNode, navigation)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this);
        ConcatenatingComposer.Instance.Compose(builder, Children.ToList());
        GroupQuantifier? groupQuantifier = IsOptional ? GroupQuantifier.Optional : null;
        builder.CloseGroup(groupQuantifier);
    }

    public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    {
        var scopedCaptureContext = captureContext[FullyQualifiedName];

        if (!scopedCaptureContext.Success)
            return null;

        var instance = (TokenUnit)Activator.CreateInstance(UnderlyingType);

        foreach (var captureNode in NamedGroupNodes)
        {
            // will return false only if an underlying property has AbortIfSetPropertyToNull == true
            // and the property value is null
            var setSuccessfully = captureNode.SetPropertyValue(scopedCaptureContext, instance);

            if (!setSuccessfully)
                return null;
        }

        CaptureValueHydrationInfo = new(this, scopedCaptureContext.Capture, instance);

        return instance;
    }
}
