namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target attacking or blocking creatures", "target creatures with flying", "target white and/or blue creatures", "target creatures and/or planeswalkers", "target creatures your opponents control": targets of a kind (or of any of several kinds), in the plural, after a count, possibly limited by who controls them.</summary>
/// <exampledoc>Deft Dismissal</exampledoc>
/// <examplecapture>target attacking or blocking creatures</examplecapture>
[Dependent]
public class TargetsOfKind : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Qualifiers), Prop(Kind), Prop(Controller), Prop(With)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, CompoundOf<TargetQualifier>> Qualifiers { get; set; }
    public OneOf<PluralKindsAndOr, PluralKind> Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
