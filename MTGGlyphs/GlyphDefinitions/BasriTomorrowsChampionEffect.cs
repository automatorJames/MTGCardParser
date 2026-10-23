namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Basri, Tomorrow's Champion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Basri, Tomorrow's Champion</exampledoc>
[Dependent]
public class BasriTomorrowsChampionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 white cat creature token with lifelink"];
}
