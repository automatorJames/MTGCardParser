namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Celestial Reunion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Celestial Reunion</exampledoc>
/// <examplecapture>if this spell's additional cost was paid and the revealed card is the chosen type, put that card onto the battlefield instead of putting it into your hand</examplecapture>
[TokenizationOrder(-1)]
public class CelestialReunionRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell's additional cost was paid and the revealed card is the chosen type, put that card onto the battlefield instead of putting it into your hand"];
}
