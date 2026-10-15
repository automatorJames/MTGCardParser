namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Replacing "target" in a copy: "if you do, change its text by replacing all instances of "target" with "each".</summary>
/// <exampledoc>Mirrorweave</exampledoc>
/// <examplecapture>if you do, change its text by replacing all instances of "target" with "each</examplecapture>
public class ChangeTextTargetToEach : Glyph
{
    public override Nib[] Nibs => ["if you do, change its text by replacing all instances of \"target\" with \"each"];
}
