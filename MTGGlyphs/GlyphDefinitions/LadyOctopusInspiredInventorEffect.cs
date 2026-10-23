namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lady Octopus, Inspired Inventor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lady Octopus, Inspired Inventor</exampledoc>
/// <examplecapture>you may cast an artifact spell from your hand with mana value less than or equal to the number of ingenuity counters on lady octopus without paying its mana cost</examplecapture>
[Dependent]
public class LadyOctopusInspiredInventorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast an artifact spell from your hand with mana value less than or equal to the number of ingenuity counters on lady octopus without paying its mana cost"];
}
