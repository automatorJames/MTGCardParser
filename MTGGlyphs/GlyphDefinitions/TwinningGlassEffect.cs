namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Twinning Glass, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Twinning Glass</exampledoc>
/// <examplecapture>you may cast a spell from your hand without paying its mana cost if it has the same name as a spell that was cast this turn</examplecapture>
[Dependent]
public class TwinningGlassEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a spell from your hand without paying its mana cost if it has the same name as a spell that was cast this turn"];
}
