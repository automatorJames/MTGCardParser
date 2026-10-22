namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "lands", "nonblack creatures", "swamps", "tapped blue creatures they control", "creatures on the battlefield", "creatures with flying", "creatures you control with power 2 or less", "creatures you control of the chosen type", "artifacts, creatures, and lands", "artifacts, legendaries, and/or sagas", "white and/or blue creatures", "creature they control that's attacking you", "creatures that share a creature type with it", "permanents other than this equipment": permanents of one kind or several (card type, land type or creature type), optionally narrowed
/// before ("nonblack") or after ("with flying", "of the chosen color", "that's attacking you", "that share a creature type with it"), optionally by who controls them, or anywhere on the battlefield, and optionally excluding this one. The kind is a referent, so a later "they" or "those creatures" refers to
/// them (see <see cref="ThatCard"/>).
/// </summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>tapped blue creatures they control</examplecapture>
[Dependent]
public class Permanents : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Prop(Kind), Prop(Controller), Prop(OtherThan), Prop(Narrowed), Prop(InCombat)];

    [Optional]
    public QualifierWords Qualifiers { get; set; }
    [Referent]
    [Plural]
    public OneOf<PluralKindsAndOr, ManyOf<PluralKind>, PluralKind> Kind { get; set; }
    [Optional]
    public OneOf<ControlledBy, OnTheBattlefield> Controller { get; set; }
    [Optional]
    public OtherThanThis OtherThan { get; set; }
    [Optional]
    public OneOf<WithQualifier, ChosenAspect?> Narrowed { get; set; }
    [Optional]
    public OneOf<ThatsInCombat, SharesCreatureType> InCombat { get; set; }
}
