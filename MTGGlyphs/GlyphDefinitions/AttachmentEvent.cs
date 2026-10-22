namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An aura or equipment becoming attached or unattached, as a trigger event: "this equipment becomes unattached from a permanent", "an aura becomes attached to this creature", "an aura you control becomes attached to a creature you control".</summary>
/// <exampledoc>Grafted Exoskeleton</exampledoc>
/// <examplecapture>this equipment becomes unattached from a permanent</examplecapture>
[Dependent]
public class AttachmentEvent : Glyph
{
    public override Nib[] Nibs => [Prop(Attachment), Prop(Change), Prop(Host)];

    public OneOf<PermanentPhrase, IndefinitePermanent> Attachment { get; set; }
    public AttachmentChange Change { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent> Host { get; set; }
}
