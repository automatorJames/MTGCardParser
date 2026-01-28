namespace MTGPlexer.TokenAnalysisDTOs.GraphAnalysis;

public record TreeNode : Node
{
    public TreeNode(TemplatePropInfo templatePropInfo, ExtractedCapture capture)  : base(templatePropInfo, capture)
    {
    }
}