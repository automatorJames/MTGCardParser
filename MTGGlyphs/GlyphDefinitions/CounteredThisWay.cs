namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"is countered this way": a spell countered by the effect just described.</summary>
/// <exampledoc>Assert Authority</exampledoc>
/// <examplecapture>is countered this way</examplecapture>
[Dependent]
public class CounteredThisWay : Glyph
{
    public override Nib[] Nibs => ["is countered this way"];
}
