namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Green Sun's Twilight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Green Sun's Twilight</exampledoc>
[TokenizationOrder(-1)]
public class GreenSunsTwilightCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a creature card and/or a land card from among them"];
}
