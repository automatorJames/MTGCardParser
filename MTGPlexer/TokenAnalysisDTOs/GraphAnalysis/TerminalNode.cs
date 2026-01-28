namespace MTGPlexer.TokenAnalysisDTOs.GraphAnalysis;

public record TerminalNode : Node
{
    public TerminalNode(TemplatePropInfo templatePropInfo, ExtractedCapture capture) : base(templatePropInfo, capture)
    {
    }
}