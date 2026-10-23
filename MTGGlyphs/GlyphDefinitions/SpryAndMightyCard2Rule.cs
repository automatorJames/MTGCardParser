namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spry and Mighty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spry and Mighty</exampledoc>
[TokenizationOrder(-1)]
public class SpryAndMightyCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose exactly two creatures you control"];
}
