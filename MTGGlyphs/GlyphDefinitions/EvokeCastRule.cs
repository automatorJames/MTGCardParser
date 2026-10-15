namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Evoke's reminder rule: the alternative cost lets you cast the spell for its evoke cost.</summary>
/// <exampledoc>Dawnfluke</exampledoc>
/// <examplecapture>you may cast this spell for its evoke cost</examplecapture>
public class EvokeCastRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this spell for its evoke cost"];
}
