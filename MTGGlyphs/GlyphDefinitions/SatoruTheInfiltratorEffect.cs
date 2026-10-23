namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Satoru, the Infiltrator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Satoru, the Infiltrator</exampledoc>
/// <examplecapture>if none of them were cast or no mana was spent to cast them, draw a card</examplecapture>
[Dependent]
public class SatoruTheInfiltratorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if none of them were cast or no mana was spent to cast them, draw a card"];
}
