namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Luxior, Giada's Gift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Luxior, Giada's Gift</exampledoc>
[Dependent]
public class LuxiorGiadasGiftEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["loyalty abilities can still be activated"];
}
