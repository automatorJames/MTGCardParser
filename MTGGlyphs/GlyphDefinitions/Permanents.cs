namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "lands", "nonblack creatures", "swamps", "tapped blue creatures they control", "creatures on the battlefield", "creatures with flying", "creatures you control with power 2 or less", "creatures you control of the chosen type", "artifacts, creatures, and lands": permanents of one kind or several (card type, land type or creature type), optionally narrowed
/// before ("nonblack") or after ("with flying", "of the chosen color"), and optionally by who controls them, or anywhere on the battlefield. The kind is a referent, so a later "they" or "those creatures" refers to
/// them (see <see cref="ThatCard"/>).
/// </summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>tapped blue creatures they control</examplecapture>
[Dependent]
public class Permanents : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Prop(Kind), Prop(Controller), Prop(Narrowed)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Referent]
    [Plural]
    public OneOf<ManyOf<PluralKind>, PluralKind> Kind { get; set; }
    [Optional]
    public OneOf<ControlledBy, OnTheBattlefield> Controller { get; set; }
    [Optional]
    public OneOf<WithQualifier, ChosenAspect?> Narrowed { get; set; }
}
