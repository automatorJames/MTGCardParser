namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glyph of Reincarnation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glyph of Reincarnation</exampledoc>
/// <examplecapture>for each creature that died this way, put a creature card from the graveyard of the player who controlled that creature the last time it became blocked by that wall onto the battlefield under its owner's control</examplecapture>
[TokenizationOrder(-1)]
public class GlyphOfReincarnationRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each creature that died this way, put a creature card from the graveyard of the player who controlled that creature the last time it became blocked by that wall onto the battlefield under its owner's control"];
}
