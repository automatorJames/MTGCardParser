namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kozilek, the Great Distortion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kozilek, the Great Distortion</exampledoc>
[Dependent]
public class KozilekTheGreatDistortionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell with mana value x"];
}
