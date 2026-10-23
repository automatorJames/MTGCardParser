namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Buster Sword, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Buster Sword</exampledoc>
/// <examplecapture>draw a card, then you may cast a spell from your hand with mana value less than or equal to that damage without paying its mana cost</examplecapture>
[Dependent]
public class BusterSwordEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card, then you may cast a spell from your hand with mana value less than or equal to that damage without paying its mana cost"];
}
