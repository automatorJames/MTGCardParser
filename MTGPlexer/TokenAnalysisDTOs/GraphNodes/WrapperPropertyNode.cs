namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public abstract record WrapperPropertyNode : CaptureNode
{
    public WrapperPropertyNode(PropertySnippet propertySnippet) : base(propertySnippet)
    {
    }
}