namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Djinn Illuminatus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Djinn Illuminatus</exampledoc>
[TokenizationOrder(-1)]
public class DjinnIlluminatusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each instant and sorcery spell you cast has replicate"];
}
