namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hedron Alignment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hedron Alignment</exampledoc>
/// <examplecapture>if you do, you win the game if you own a card named {this} in exile, in your hand, in your graveyard, and on the battlefield</examplecapture>
[TokenizationOrder(-1)]
public class HedronAlignmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, you win the game if you own a card named", Nib.This, "in exile, in your hand, in your graveyard, and on the battlefield"];
}
