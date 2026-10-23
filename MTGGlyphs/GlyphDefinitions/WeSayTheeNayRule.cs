namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of We Say Thee Nay!, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>We Say Thee Nay!</exampledoc>
[TokenizationOrder(-1)]
public class WeSayTheeNayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter that spell unless its controller pays {4} instead if this spell was cast using teamwork"];
}
