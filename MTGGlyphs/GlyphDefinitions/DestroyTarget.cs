namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"destroy target land", "destroy {this}", "destroy that creature": destroys one permanent.</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>destroy target land</examplecapture>
public class DestroyTarget : Glyph
{
    public override Nib[] Nibs => ["destroy", Prop(Destroyed)];

    public PermanentPhrase Destroyed { get; set; }
}
