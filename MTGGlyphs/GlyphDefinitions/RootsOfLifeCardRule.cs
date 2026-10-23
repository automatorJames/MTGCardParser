namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Roots of Life, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Roots of Life</exampledoc>
[TokenizationOrder(-1)]
public class RootsOfLifeCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a land of the chosen type an opponent controls becomes tapped, you gain 1 life"];
}
