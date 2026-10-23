namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Goliath Daydreamer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goliath Daydreamer</exampledoc>
/// <examplecapture>you may cast a spell from among cards you own in exile with dream counters on them without paying its mana cost</examplecapture>
[Dependent]
public class GoliathDaydreamer2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a spell from among cards you own in exile with dream counters on them without paying its mana cost"];
}
