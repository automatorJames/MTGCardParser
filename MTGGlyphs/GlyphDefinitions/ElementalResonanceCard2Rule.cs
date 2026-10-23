namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elemental Resonance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elemental Resonance</exampledoc>
[TokenizationOrder(-1)]
public class ElementalResonanceCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a mana symbol has multiple colors, choose one"];
}
