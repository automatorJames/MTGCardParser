namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>Represents <see cref="UnmatchedString"/>, the fallback token type for source text that matched no other <see cref="Glyph"/>.</summary>
public class UnmatchedGlyphNode : GlyphNode
{
    public UnmatchedGlyphNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
    }
}
