namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Unpredictable Cyclone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unpredictable Cyclone</exampledoc>
/// <examplecapture>if a cycling ability of another nonland card would cause you to draw a card, instead exile cards from the top of your library until you exile a card that shares a card type with the cycled card</examplecapture>
[TokenizationOrder(-1)]
public class UnpredictableCycloneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a cycling ability of another nonland card would cause you to draw a card, instead exile cards from the top of your library until you exile a card that shares a card type with the cycled card"];
}
