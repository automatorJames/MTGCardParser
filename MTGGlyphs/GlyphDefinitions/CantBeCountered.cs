namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell that can't be countered: "this spell can't be countered".</summary>
/// <exampledoc>Abrupt Decay</exampledoc>
/// <examplecapture>this spell can't be countered</examplecapture>
public class CantBeCountered : Glyph
{
    public override Nib[] Nibs => ["this spell can't be countered"];
}
