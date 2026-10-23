namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pit of Offerings, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pit of Offerings</exampledoc>
[Dependent]
public class PitOfOfferingsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add one mana of any of the exiled cards' colors"];
}
