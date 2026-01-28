namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record TreeNode : Node
{
    public TreeNode(TemplatePropInfo templatePropInfo, ExtractedCapture capture)  : base(templatePropInfo, capture)
    {
    }
}