namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Basri, Tomorrow's Champion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Basri, Tomorrow's Champion</exampledoc>
[TokenizationOrder(-1)]
public class BasriTomorrowsChampionCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{w}, {t}, exert basri: create a 1/1 white cat creature token with lifelink"];
}
