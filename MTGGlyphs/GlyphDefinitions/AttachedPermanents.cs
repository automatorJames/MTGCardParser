namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Permanents attached to another, as counted: "aura attached to it", "equipment attached to this creature", "aura and equipment attached to it".</summary>
/// <exampledoc>Evershrike</exampledoc>
/// <examplecapture>aura attached to it</examplecapture>
[Dependent]
public class AttachedPermanents : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), "attached to", Prop(Host)];

    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kind { get; set; }
    public PermanentPhrase Host { get; set; }
}
