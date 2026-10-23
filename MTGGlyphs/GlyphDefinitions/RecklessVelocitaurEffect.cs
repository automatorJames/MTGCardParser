namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Reckless Velocitaur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reckless Velocitaur</exampledoc>
/// <examplecapture>that mount or vehicle gets +2/+0 and gains trample until end of turn</examplecapture>
[Dependent]
public class RecklessVelocitaurEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that mount or vehicle gets +2/+0 and gains trample until end of turn"];
}
