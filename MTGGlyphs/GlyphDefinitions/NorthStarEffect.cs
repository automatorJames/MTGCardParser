namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of North Star, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>North Star</exampledoc>
[Dependent]
public class NorthStarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for one spell this turn, you may spend mana as though it were mana of any type to pay that spell's mana cost"];
}
