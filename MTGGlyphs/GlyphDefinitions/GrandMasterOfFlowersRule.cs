namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grand Master of Flowers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grand Master of Flowers</exampledoc>
[TokenizationOrder(-1)]
public class GrandMasterOfFlowersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as", Nib.This, "has seven or more loyalty counters on him, he's a 7/7 dragon god creature with flying and indestructible"];
}
