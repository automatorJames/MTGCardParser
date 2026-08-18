namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class TokenUnitOneOfNode : TokenUnitNode
{
    public override CaptureNodeKind NodeKind => CaptureNodeKind.OneOf;
    public TokenUnitOneOfNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
    }

    public override bool TryHydrate(CaptureTrace captureTrace, out CaptureUnit tokenUnit)
    {
        tokenUnit = null;
        var instance = (CaptureUnit)Activator.CreateInstance(Navigation.NodeType);

        // Counter to track successfully set children
        int childrenSuccessfullySet = 0;

        foreach (var child in NamedGroupChildren)
        {
            var setResult = child.SetPropertyValue(instance, captureTrace.CaptureContext);

            if (setResult)
                childrenSuccessfullySet++;
        }

        // In a one-of node, we expect exactly one child to match
        if (childrenSuccessfullySet != 1)
            return false;

        instance.CaptureContext = captureTrace.CaptureContext;
        tokenUnit = instance;

        return true;
    }
}