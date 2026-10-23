namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Any one permanent of a kind, as the host of an attachment: "a creature", "a permanent", "a creature you control" - in "that's attached to a creature". Cycle-free (no with-qualifiers), so permanent descriptors can use it.</summary>
/// <exampledoc>Sage's Reverie</exampledoc>
/// <examplecapture>a creature</examplecapture>
[Dependent]
public class AttachmentHostKind : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Kind), Prop(Controller)];

    public PermanentKind Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
}
