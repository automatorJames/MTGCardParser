namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tifa's Limit Break, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tifa's Limit Break</exampledoc>
[TokenizationOrder(-1)]
public class TifasLimitBreakRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• final heaven — {6}{g} — triple target creature's power and toughness until end of turn"];
}
