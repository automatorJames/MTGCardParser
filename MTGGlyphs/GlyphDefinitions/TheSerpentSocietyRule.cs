namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Serpent Society, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Serpent Society</exampledoc>
[TokenizationOrder(-1)]
public class TheSerpentSocietyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ward—get five poison counters"];
}
