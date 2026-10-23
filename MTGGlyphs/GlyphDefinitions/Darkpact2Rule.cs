namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Darkpact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Darkpact</exampledoc>
[TokenizationOrder(-1)]
public class Darkpact2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you own target card in the ante"];
}
