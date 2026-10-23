namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Masako the Humorless, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Masako the Humorless</exampledoc>
[TokenizationOrder(-1)]
public class MasakoTheHumorlessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tapped creatures you control can block as though they were untapped"];
}
