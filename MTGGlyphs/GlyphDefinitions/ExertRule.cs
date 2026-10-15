namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exert's reminder text.</summary>
/// <exampledoc>Ahn-Crop Crasher</exampledoc>
/// <examplecapture>an exerted creature won't untap during your next untap step</examplecapture>
public class ExertRule : Glyph
{
    public override Nib[] Nibs => ["an exerted creature won't untap during your next untap step"];
}
