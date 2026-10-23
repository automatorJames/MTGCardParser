namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Koh, the Face Stealer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Koh, the Face Stealer</exampledoc>
[TokenizationOrder(-1)]
public class KohTheFaceStealerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["koh has all activated and triggered abilities of the last chosen card"];
}
