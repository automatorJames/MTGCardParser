namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Captain America, Living Legend, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Captain America, Living Legend</exampledoc>
[Dependent]
public class CaptainAmericaLivingLegendEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's the first time that creature has become tapped this turn, untap it"];
}
