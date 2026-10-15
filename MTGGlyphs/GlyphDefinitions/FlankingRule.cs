namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Flanking's reminder text.</summary>
/// <exampledoc>Bogardan Lancer</exampledoc>
/// <examplecapture>whenever a creature without flanking blocks this creature, the blocking creature gets -1/-1 until end of turn</examplecapture>
public class FlankingRule : Glyph
{
    public override Nib[] Nibs => ["whenever a creature without flanking blocks this creature, the blocking creature gets -1/-1 until end of turn"];
}
