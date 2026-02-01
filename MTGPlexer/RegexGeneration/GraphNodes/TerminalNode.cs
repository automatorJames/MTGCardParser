namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class TerminalNode : CaptureNode
{
    public TerminalNode(Node parentNode, INavigable navigable) 
        : base(parentNode, navigable)
    {
    }

    public ScalarAlternateSet ScalarAlternateSet
    {
        get
        {
            // todo: reimplement cache if appropriate
            //if (!TokenTypeRegistry.PropScalarAlternativeSets.TryGetValue(new TemplatePropInfo(PropertySnippet.Prop), out var scalarAlternativeSet))
            //{
            //    scalarAlternativeSet =  GetScalarAlternateSet();
            //    TokenTypeRegistry.PropScalarAlternativeSets[new TemplatePropInfo(PropertySnippet.Prop)] = scalarAlternativeSet;
            //}
            //
            //return scalarAlternativeSet;
            return GetScalarAlternateSet();
        }
    }

    protected virtual ScalarAlternateSet GetScalarAlternateSet()
    {
        var captureAlternatives = (OverrideRegexPatterns ?? [Navigable.Name])
            .OrderByDescending(s => s.Length)
            .ToList();

        return new(captureAlternatives);
    }

    public override CaptureValueInfo GetCaptureValueInfo(CaptureDictionary captureDictionary)
    {
        var capture = captureDictionary[FullyQualifiedName].SingleOrDefault();

        if (capture == null)
            return new(this, CaptureValueResult.NameNotFound);

        var value = TryGetValue(capture, out var result);

        if (result != CaptureValueResult.FoundWithValue)
            return new(this, result);

        return new(this, value, FullyQualifiedName, capture);
    }

    protected abstract object TryGetValue(Capture capture, out CaptureValueResult result);
}