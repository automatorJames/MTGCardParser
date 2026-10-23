namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eliminate the Impossible, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eliminate the Impossible</exampledoc>
[TokenizationOrder(-1)]
public class EliminateTheImpossibleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any of them are suspected, they're no longer suspected"];
}
