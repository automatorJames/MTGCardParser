namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Artificial Evolution, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Artificial Evolution</exampledoc>
[TokenizationOrder(-1)]
public class ArtificialEvolutionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the new creature type can't be wall"];
}
