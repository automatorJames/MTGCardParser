namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jaded Response, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jaded Response</exampledoc>
[TokenizationOrder(-1)]
public class JadedResponseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell if it shares a color with a creature you control"];
}
