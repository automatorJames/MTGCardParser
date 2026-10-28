namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Frantic Scapegoat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Frantic Scapegoat</exampledoc>
[TokenizationOrder(-1)]
public class FranticScapegoatRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, this creature is no longer suspected"];
}
