namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target beyond the first" (strive): each extra target chosen for a spell; or plain "target", each target it has (Hinata, Phyrexian Purge).</summary>
/// <exampledoc>Harness by Force</exampledoc>
/// <examplecapture>target beyond the first</examplecapture>
[Dependent]
public class TargetBeyondFirst : Glyph
{
    public override Nib[] Nibs => ["target", Prop(BeyondFirst)];

    [Optional]
    [RegexPattern("beyond the first")]
    public bool? BeyondFirst { get; set; }
}
