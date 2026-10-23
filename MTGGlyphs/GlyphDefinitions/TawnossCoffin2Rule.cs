namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tawnos's Coffin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tawnos's Coffin</exampledoc>
[TokenizationOrder(-1)]
public class TawnossCoffin2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, return the other exiled cards to the battlefield under their owner's control attached to that permanent"];
}
