namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Forget, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Forget</exampledoc>
[TokenizationOrder(-1)]
public class ForgetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player discards two cards, then draws as many cards as they discarded this way"];
}
