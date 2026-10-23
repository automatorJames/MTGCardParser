namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Celestial Dawn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Celestial Dawn</exampledoc>
[TokenizationOrder(-1)]
public class CelestialDawnRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may spend other mana only as though it were colorless mana"];
}
