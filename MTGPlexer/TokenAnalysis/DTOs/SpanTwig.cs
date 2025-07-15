namespace MTGPlexer.TokenAnalysis.DTOs;

public record SpanTwig
(
    TokenUnit Token,
    string Path,
    int NestedDepth,
    string Text
) 
: SpanTerminal(Path, NestedDepth, Text, TokenTypeRegistry.TypeColorPalettes[Token.Type], Token.Type.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null)
{
    public override string ToString() => Text;
}
