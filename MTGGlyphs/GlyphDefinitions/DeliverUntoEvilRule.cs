namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Deliver Unto Evil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deliver Unto Evil</exampledoc>
[TokenizationOrder(-1)]
public class DeliverUntoEvilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["leave the chosen cards in your graveyard and put the rest into your hand"];
}
