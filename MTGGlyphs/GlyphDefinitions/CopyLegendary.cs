namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A copy made legendary or not: "except it isn't legendary", "except it's legendary".</summary>
/// <exampledoc>Impostor Syndrome</exampledoc>
/// <examplecapture>except it isn't legendary</examplecapture>
[Dependent]
public class CopyLegendary : Glyph
{
    public LegendaryChange Change { get; set; }
}
