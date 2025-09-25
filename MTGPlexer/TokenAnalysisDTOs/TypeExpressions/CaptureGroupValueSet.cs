namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record CaptureGroupValueSet
{
    public string PropPath { get; set; }
    public string TerminalName { get; set; }
    public Dictionary<CaptureValueVariantSet, RegexCommentedAlternateLine> VariantSetToLineMap { get; set; }
}

