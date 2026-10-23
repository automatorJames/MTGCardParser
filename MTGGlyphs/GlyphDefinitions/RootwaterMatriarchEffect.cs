namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rootwater Matriarch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rootwater Matriarch</exampledoc>
[Dependent]
public class RootwaterMatriarchEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target creature for as long as that creature is enchanted"];
}
