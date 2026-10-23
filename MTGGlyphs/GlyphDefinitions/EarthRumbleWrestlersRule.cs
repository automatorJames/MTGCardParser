namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Earth Rumble Wrestlers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Earth Rumble Wrestlers</exampledoc>
/// <examplecapture>this creature gets +1/+0 and has trample as long as you control a land creature or a land entered the battlefield under your control this turn</examplecapture>
[TokenizationOrder(-1)]
public class EarthRumbleWrestlersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +1/+0 and has trample as long as you control a land creature or a land entered the battlefield under your control this turn"];
}
