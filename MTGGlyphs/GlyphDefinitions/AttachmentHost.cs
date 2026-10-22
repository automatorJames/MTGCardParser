namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What something is attached to, as a kind: "a creature", "a creature or land", "a permanent", "a creature you control". Kept small (no qualifiers) so the targets using it stay cheap to build.</summary>
/// <exampledoc>Devout Harpist</exampledoc>
/// <examplecapture>a creature</examplecapture>
[Dependent]
public class AttachmentHost : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Kind), Prop(Controller)];

    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
}
