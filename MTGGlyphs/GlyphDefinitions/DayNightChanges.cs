namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The day/night cycle turning over, as a trigger condition: "day becomes night or night becomes day".</summary>
/// <exampledoc>Firmament Sage</exampledoc>
/// <examplecapture>day becomes night or night becomes day</examplecapture>
[Dependent]
public class DayNightChanges : Glyph
{
    public override Nib[] Nibs => ["day becomes night or night becomes day"];
}
