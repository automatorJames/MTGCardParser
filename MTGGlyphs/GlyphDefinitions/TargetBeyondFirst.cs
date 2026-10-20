namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target beyond the first" (strive): each extra target chosen for a spell.</summary>
/// <exampledoc>Harness by Force</exampledoc>
/// <examplecapture>target beyond the first</examplecapture>
[Dependent]
public class TargetBeyondFirst : Glyph
{
    public override Nib[] Nibs => ["target beyond the first"];
}
