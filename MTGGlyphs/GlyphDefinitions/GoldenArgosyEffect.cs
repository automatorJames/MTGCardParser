namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Golden Argosy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Golden Argosy</exampledoc>
[Dependent]
public class GoldenArgosyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile each creature that crewed it this turn"];
}
