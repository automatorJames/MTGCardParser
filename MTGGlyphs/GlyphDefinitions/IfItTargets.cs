namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"if it targets a creature you control": a cost condition on what the spell targets.</summary>
/// <exampledoc>Into the Roil</exampledoc>
/// <examplecapture>if it targets a creature you control</examplecapture>
[Dependent]
public class IfItTargets : Glyph
{
    public override Nib[] Nibs => ["if it targets", Prop(Target)];

    public OneOf<IndefinitePermanent, PermanentPhrase> Target { get; set; }
}
