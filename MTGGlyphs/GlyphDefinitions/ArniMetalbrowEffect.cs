namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Arni Metalbrow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Arni Metalbrow</exampledoc>
[Dependent]
public class ArniMetalbrowEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay {1}{r}"];
}
