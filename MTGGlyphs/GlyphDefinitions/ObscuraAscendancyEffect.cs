namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Obscura Ascendancy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Obscura Ascendancy</exampledoc>
/// <examplecapture>if its mana value is equal to 1 plus the number of soul counters on this enchantment, put a soul counter on this enchantment, then create a 2/2 white spirit creature token with flying</examplecapture>
[Dependent]
public class ObscuraAscendancyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if its mana value is equal to 1 plus the number of soul counters on this enchantment, put a soul counter on this enchantment, then create a 2/2 white spirit creature token with flying"];
}
