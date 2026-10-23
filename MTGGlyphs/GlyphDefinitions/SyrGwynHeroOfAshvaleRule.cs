namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Syr Gwyn, Hero of Ashvale, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Syr Gwyn, Hero of Ashvale</exampledoc>
[TokenizationOrder(-1)]
public class SyrGwynHeroOfAshvaleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipment you control have equip knight {0}"];
}
