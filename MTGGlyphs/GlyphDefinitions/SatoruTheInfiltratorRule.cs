namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Satoru, the Infiltrator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Satoru, the Infiltrator</exampledoc>
[TokenizationOrder(-1)]
public class SatoruTheInfiltratorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever satoru and/or one or more other nontoken creatures you control enter, if none of them were cast or no mana was spent to cast them, draw a card"];
}
