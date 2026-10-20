namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A becoming effect that adds rather than replaces: "it keeps its abilities and types".</summary>
/// <exampledoc>Mishra's Factory</exampledoc>
/// <examplecapture>it keeps its abilities and types</examplecapture>
public class KeepsAbilitiesAndTypes : Glyph
{
    public override Nib[] Nibs => [Alt("it keeps", "they keep"), "its abilities and types"];
}
