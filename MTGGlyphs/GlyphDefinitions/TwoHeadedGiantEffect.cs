namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Two-Headed Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Two-Headed Giant</exampledoc>
[Dependent]
public class TwoHeadedGiantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip two coins"];
}
