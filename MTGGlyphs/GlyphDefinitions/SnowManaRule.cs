namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Snow mana's reminder text: "{s} can be paid with one mana from a snow source", or, where {s} counts mana spent, "{s} is mana from a snow source".</summary>
/// <exampledoc>Rimewind Taskmage</exampledoc>
/// <examplecapture>{s} can be paid with one mana from a snow source</examplecapture>
public class SnowManaRule : Glyph
{
    public override Nib[] Nibs => ["{s}", Alt("can be paid with one", "is"), "mana from a snow source"];
}
