namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract record TerminalNode : CaptureNode
{
    public TerminalNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
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
        var captureAlternatives = (PropertySnippet.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [PropertySnippet.Prop.Name]).OrderByDescending(s => s.Length).ToList();
        return new(captureAlternatives);
    }
}