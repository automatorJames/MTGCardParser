namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Foil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Foil</exampledoc>
[TokenizationOrder(-1)]
public class FoilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may discard an island card and another card rather than pay this spell's mana cost"];
}
