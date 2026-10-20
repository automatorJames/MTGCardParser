namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cases' reminder: "if unsolved, solve at the beginning of your end step".</summary>
/// <exampledoc>Case of the Burning Masks</exampledoc>
/// <examplecapture>if unsolved, solve at the beginning of your end step</examplecapture>
public class CaseSolveRule : Glyph
{
    public override Nib[] Nibs => ["if unsolved, solve at the beginning of your end step"];
}
