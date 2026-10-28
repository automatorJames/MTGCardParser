namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hivis of the Scale, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hivis of the Scale</exampledoc>
[TokenizationOrder(-1)]
public class HivisOfTheScaleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may choose not to untap hivis during your untap step"];
}
