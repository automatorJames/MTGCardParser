namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mechtitan Core, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mechtitan Core</exampledoc>
/// <examplecapture>{5}, exile this vehicle and four other artifact creatures and/or vehicles you control: create mechtitan, a legendary 10/10 construct artifact creature token with flying, vigilance, trample, lifelink, and haste that's all colors</examplecapture>
[TokenizationOrder(-1)]
public class MechtitanCoreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{5}, exile this vehicle and four other artifact creatures and/or vehicles you control: create mechtitan, a legendary 10/10 construct artifact creature token with flying, vigilance, trample, lifelink, and haste that's all colors"];
}
