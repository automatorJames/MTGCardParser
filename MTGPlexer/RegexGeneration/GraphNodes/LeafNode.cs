namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class LeafNode : SingleCaptureNode
{
    public LeafNode(Node parentNode, INavigable navigable) 
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
}