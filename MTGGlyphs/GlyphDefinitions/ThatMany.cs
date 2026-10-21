namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that many" / "any amount of": an unwritten number of things.</summary>
/// <exampledoc>Empyreal Voyager</exampledoc>
/// <examplecapture>that many</examplecapture>
[Dependent]
public class ThatMany : Glyph
{
    public override Nib[] Nibs => [Alt("that many", "any amount of")];
}
