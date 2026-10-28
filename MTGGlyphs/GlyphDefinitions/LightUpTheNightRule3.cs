namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Light Up the Night, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Light Up the Night</exampledoc>
[TokenizationOrder(-1)]
public class LightUpTheNightRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast this spell this way, x can't be 0"];
}
