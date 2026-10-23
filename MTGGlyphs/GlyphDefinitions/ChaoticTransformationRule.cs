namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chaotic Transformation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chaotic Transformation</exampledoc>
/// <examplecapture>for each permanent exiled this way, its controller reveals cards from the top of their library until they reveal a card that shares a card type with it, puts that card onto the battlefield, then shuffles</examplecapture>
[TokenizationOrder(-1)]
public class ChaoticTransformationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each permanent exiled this way, its controller reveals cards from the top of their library until they reveal a card that shares a card type with it, puts that card onto the battlefield, then shuffles"];
}
