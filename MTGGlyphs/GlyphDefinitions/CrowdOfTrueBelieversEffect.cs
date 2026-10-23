namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Crowd of True Believers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crowd of True Believers</exampledoc>
[Dependent]
public class CrowdOfTrueBelieversEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature you control that's attacking alone gets +1/+0 until end of turn"];
}
