namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Planetarium of Wan Shi Tong, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Planetarium of Wan Shi Tong</exampledoc>
[Dependent]
public class PlanetariumOfWanShiTongEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the card after you scry or surveil"];
}
