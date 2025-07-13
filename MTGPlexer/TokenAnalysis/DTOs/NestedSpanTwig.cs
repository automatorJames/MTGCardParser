namespace MTGPlexer.TokenAnalysis.DTOs;

public record NestedSpanTwig
(
    TokenUnit Token,
    string Path,
    int NestedDepth,
    string Text
) 
: NestedSpanTerminal(Path, NestedDepth, Text, TokenTypeRegistry.TypeColorPalettes[Token.Type])
{
    public override string ToString() => Text;
}
