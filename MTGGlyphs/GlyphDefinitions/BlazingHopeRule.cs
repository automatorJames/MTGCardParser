namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blazing Hope, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blazing Hope</exampledoc>
[TokenizationOrder(-1)]
public class BlazingHopeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target creature with power greater than or equal to your life total"];
}
