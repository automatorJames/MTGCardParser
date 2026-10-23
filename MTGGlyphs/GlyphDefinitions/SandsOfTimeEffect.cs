namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sands of Time, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sands of Time</exampledoc>
/// <examplecapture>that player simultaneously untaps each tapped artifact, creature, and land they control and taps each untapped artifact, creature, and land they control</examplecapture>
[Dependent]
public class SandsOfTimeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player simultaneously untaps each tapped artifact, creature, and land they control and taps each untapped artifact, creature, and land they control"];
}
