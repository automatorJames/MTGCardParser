namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Snow mana's reminder text.</summary>
/// <exampledoc>Rimewind Taskmage</exampledoc>
/// <examplecapture>{s} can be paid with one mana from a snow source</examplecapture>
public class SnowManaRule : Glyph
{
    public override Nib[] Nibs => ["{s} can be paid with one mana from a snow source"];
}
