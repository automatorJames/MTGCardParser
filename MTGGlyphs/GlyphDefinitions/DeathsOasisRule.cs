namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Death's Oasis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Death's Oasis</exampledoc>
[TokenizationOrder(-1)]
public class DeathsOasisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then return a creature card with lesser mana value than the creature that died from your graveyard to your hand"];
}
