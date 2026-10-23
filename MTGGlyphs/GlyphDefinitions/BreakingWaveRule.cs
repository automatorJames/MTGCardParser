namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Breaking Wave, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Breaking Wave</exampledoc>
[TokenizationOrder(-1)]
public class BreakingWaveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["simultaneously untap all tapped creatures and tap all untapped creatures"];
}
