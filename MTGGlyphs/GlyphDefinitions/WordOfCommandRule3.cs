namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Word of Command, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Word of Command</exampledoc>
/// <examplecapture>while doing so, the player can activate mana abilities only if they're from lands that player controls and only if mana they produce is spent to activate other mana abilities of lands the player controls and/or to play that card</examplecapture>
[TokenizationOrder(-1)]
public class WordOfCommandRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["while doing so, the player can activate mana abilities only if they're from lands that player controls and only if mana they produce is spent to activate other mana abilities of lands the player controls and/or to play that card"];
}
