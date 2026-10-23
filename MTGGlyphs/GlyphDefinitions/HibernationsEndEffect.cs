namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hibernation's End, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hibernation's End</exampledoc>
/// <examplecapture>you may search your library for a creature card with mana value equal to the number of age counters on this enchantment, put it onto the battlefield, then shuffle</examplecapture>
[Dependent]
public class HibernationsEndEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your library for a creature card with mana value equal to the number of age counters on this enchantment, put it onto the battlefield, then shuffle"];
}
