namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ral Zarek, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ral Zarek</exampledoc>
[TokenizationOrder(-1)]
public class RalZarekRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["take an extra turn after this one for each coin that comes up heads"];
}
