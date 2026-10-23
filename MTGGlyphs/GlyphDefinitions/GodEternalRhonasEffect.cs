namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of God-Eternal Rhonas, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>God-Eternal Rhonas</exampledoc>
[Dependent]
public class GodEternalRhonasEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["double the power of each other creature you control until end of turn"];
}
