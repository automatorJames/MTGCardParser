namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Descent into Madness, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Descent into Madness</exampledoc>
/// <examplecapture>put a despair counter on this enchantment, then each player exiles x permanents they control and/or cards from their hand, where x is the number of despair counters on this enchantment</examplecapture>
[Dependent]
public class DescentIntoMadnessEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a despair counter on this enchantment, then each player exiles x permanents they control and/or cards from their hand, where x is the number of despair counters on this enchantment"];
}
