namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Market Gnome, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Market Gnome</exampledoc>
[Dependent]
public class MarketGnomeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 1 life and draw a card"];
}
