namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Skrelv, Defector Mite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skrelv, Defector Mite</exampledoc>
[TokenizationOrder(-1)]
public class SkrelvDefectorMiteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can't be blocked by creatures of that color this turn"];
}
