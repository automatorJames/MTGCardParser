namespace MTGPlexer.TokenAnalysis.MatchDTOs;

public record SpanTwig
(
    TokenUnit Token,
    string Path,
    int NestedDepth,
    string Text
) 
: SpanTerminal(Path, NestedDepth, Text, TokenTypeRegistry.Palettes[Token.Type], Token.Type.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null)
{
    public override string ToString() => Text;
}
