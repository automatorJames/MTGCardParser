namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Glyph of Life, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glyph of Life</exampledoc>
[Dependent]
public class GlyphOfLifeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain that much life"];
}
