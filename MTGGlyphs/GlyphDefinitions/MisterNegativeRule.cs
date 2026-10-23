namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mister Negative, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mister Negative</exampledoc>
[TokenizationOrder(-1)]
public class MisterNegativeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lost life this way, draw that many cards"];
}
