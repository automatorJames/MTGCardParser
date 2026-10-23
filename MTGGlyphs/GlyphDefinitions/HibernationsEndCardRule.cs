namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hibernation's End, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hibernation's End</exampledoc>
[TokenizationOrder(-1)]
public class HibernationsEndCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you pay this enchantment's cumulative upkeep, you may search your library for a creature card with mana value equal to the number of age counters on this enchantment, put it onto the battlefield, then shuffle"];
}
