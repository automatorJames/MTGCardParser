namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of See the Truth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>See the Truth</exampledoc>
[TokenizationOrder(-1)]
public class SeeTheTruthRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was cast from anywhere other than your hand, put each of those cards into your hand instead"];
}
