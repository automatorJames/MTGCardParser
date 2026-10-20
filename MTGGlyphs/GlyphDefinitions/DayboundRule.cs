namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Daybound's reminder text.</summary>
/// <exampledoc>Curse of Leeches // Leeching Lurker</exampledoc>
/// <examplecapture>if a player casts no spells during their own turn, it becomes night next turn</examplecapture>
public class DayboundRule : Glyph
{
    public override Nib[] Nibs => ["if a player casts no spells during their own turn, it becomes night next turn"];
}
