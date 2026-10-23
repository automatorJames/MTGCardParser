namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hama, the Bloodbender, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hama, the Bloodbender</exampledoc>
/// <examplecapture>for as long as you control hama, you may cast the exiled card during your turn by waterbending {x} rather than paying its mana cost, where x is its mana value</examplecapture>
[TokenizationOrder(-1)]
public class HamaTheBloodbenderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for as long as you control hama, you may cast the exiled card during your turn by waterbending {x} rather than paying its mana cost, where x is its mana value"];
}
