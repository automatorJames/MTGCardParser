namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaboom!, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaboom!</exampledoc>
/// <examplecapture>for each of them, reveal cards from the top of your library until you reveal a nonland card, {this} deals damage equal to that card's mana value to that player or planeswalker, then you put the revealed cards on the bottom of your library in any order</examplecapture>
[TokenizationOrder(-1)]
public class KaboomRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each of them, reveal cards from the top of your library until you reveal a nonland card,", Nib.This, "deals damage equal to that card's mana value to that player or planeswalker, then you put the revealed cards on the bottom of your library in any order"];
}
