namespace MTGPlexer.TokenAnalysisDTOs.GraphAnalysis;

public record BranchNode : Node
{
    public BranchNode(TemplatePropInfo templatePropInfo, ExtractedCapture capture) : base(templatePropInfo, capture)
    {
    }
}