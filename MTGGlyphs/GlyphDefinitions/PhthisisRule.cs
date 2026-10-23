namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Phthisis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phthisis</exampledoc>
[TokenizationOrder(-1)]
public class PhthisisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its controller loses life equal to its power plus its toughness"];
}
