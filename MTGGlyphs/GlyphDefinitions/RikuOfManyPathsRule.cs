namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Riku of Many Paths, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riku of Many Paths</exampledoc>
[TokenizationOrder(-1)]
public class RikuOfManyPathsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast a modal spell, choose up to x, where x is the number of times you chose a mode for that spell —"];
}
