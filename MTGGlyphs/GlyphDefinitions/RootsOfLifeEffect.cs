namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Roots of Life, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Roots of Life</exampledoc>
[Dependent]
public class RootsOfLifeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 1 life"];
}
