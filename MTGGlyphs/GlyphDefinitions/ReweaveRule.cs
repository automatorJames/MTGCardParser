namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reweave, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reweave</exampledoc>
/// <examplecapture>if the player does, they reveal cards from the top of their library until they reveal a permanent card that shares a card type with the sacrificed permanent, put that card onto the battlefield, then shuffle</examplecapture>
[TokenizationOrder(-1)]
public class ReweaveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the player does, they reveal cards from the top of their library until they reveal a permanent card that shares a card type with the sacrificed permanent, put that card onto the battlefield, then shuffle"];
}
