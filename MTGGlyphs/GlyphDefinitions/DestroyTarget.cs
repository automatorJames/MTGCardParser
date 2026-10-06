namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"destroy target land": destroys one targeted permanent.</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>destroy target land</examplecapture>
public class DestroyTarget : Glyph
{
    public override Nib[] Nibs => ["destroy", Prop(Destroyed)];

    public TargetPermanent Destroyed { get; set; }
}
