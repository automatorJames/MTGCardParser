
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
            if (!TokenTypeRegistry.ScalarAlternateSetCache.TryGetValue(UnderlyingType, out var scalarAlternativeSet))
            {
                scalarAlternativeSet =  GetScalarAlternateSet();
                TokenTypeRegistry.ScalarAlternateSetCache[UnderlyingType] = scalarAlternativeSet;
            }
            
            return scalarAlternativeSet;
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