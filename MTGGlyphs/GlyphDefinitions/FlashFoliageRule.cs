namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flash Foliage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flash Foliage</exampledoc>
[TokenizationOrder(-1)]
public class FlashFoliageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 green saproling creature token that's blocking target creature attacking you"];
}
