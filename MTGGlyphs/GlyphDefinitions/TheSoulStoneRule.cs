namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Soul Stone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Soul Stone</exampledoc>
[TokenizationOrder(-1)]
public class TheSoulStoneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["∞ — at the beginning of your upkeep, return target creature card from your graveyard to the battlefield"];
}
