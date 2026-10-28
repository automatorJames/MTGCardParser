namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jeweled Bird, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jeweled Bird</exampledoc>
[Dependent]
public class JeweledBirdEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ante this artifact"];
}
