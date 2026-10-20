namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Earthbend's reminder text, first sentence (the counters sentence parses as PlaceCounters).</summary>
/// <exampledoc>Badgermole</exampledoc>
/// <examplecapture>target land you control becomes a 0/0 creature with haste that's still a land</examplecapture>
public class EarthbendRule : Glyph
{
    public override Nib[] Nibs => ["target land you control becomes a 0/0 creature with haste that's still a land"];
}
