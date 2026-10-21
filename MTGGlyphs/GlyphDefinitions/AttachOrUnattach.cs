namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reconfigure's effect: attaching to a creature, or unattaching from one - "attach to target creature you control; or unattach from a creature".</summary>
/// <exampledoc>Simian Sling</exampledoc>
/// <examplecapture>attach to target creature you control; or unattach from a creature</examplecapture>
public class AttachOrUnattach : Glyph
{
    public override Nib[] Nibs => [Prop(Attach), "; or unattach from", Prop(From)];

    public AttachToTarget Attach { get; set; }
    public IndefinitePermanent From { get; set; }
}
