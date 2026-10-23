namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Riptide Mangler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riptide Mangler</exampledoc>
[Dependent]
public class RiptideManglerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature's base power becomes equal to target creature's power"];
}
