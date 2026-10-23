namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Master of Winds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master of Winds</exampledoc>
[TokenizationOrder(-1)]
public class MasterOfWindsCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast an instant, sorcery, or wizard spell, you may have this creature's base power and toughness become 4/1 or 1/4 until end of turn"];
}
