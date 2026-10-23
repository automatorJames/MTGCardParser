namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Odds // Ends, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Odds // Ends</exampledoc>
[TokenizationOrder(-1)]
public class OddsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it comes up heads, counter target instant or sorcery spell"];
}
