namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Coat of Arms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coat of Arms</exampledoc>
[TokenizationOrder(-1)]
public class CoatOfArmsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature gets +1/+1 for each other creature on the battlefield that shares at least one creature type with it"];
}
