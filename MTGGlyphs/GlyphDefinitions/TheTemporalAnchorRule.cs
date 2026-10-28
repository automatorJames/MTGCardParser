namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Temporal Anchor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Temporal Anchor</exampledoc>
[TokenizationOrder(-1)]
public class TheTemporalAnchorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you choose to put one or more cards on the bottom of your library while scrying, exile that many cards from the bottom of your library"];
}
