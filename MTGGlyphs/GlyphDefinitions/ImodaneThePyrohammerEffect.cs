namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Imodane, the Pyrohammer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imodane, the Pyrohammer</exampledoc>
/// <examplecapture>imodane deals that much damage to each opponent</examplecapture>
[Dependent]
public class ImodaneThePyrohammerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["imodane deals that much damage to each opponent"];
}
