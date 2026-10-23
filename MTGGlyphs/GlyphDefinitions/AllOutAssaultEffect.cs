namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of All-Out Assault, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>All-Out Assault</exampledoc>
[Dependent]
public class AllOutAssaultEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap each creature you control"];
}
