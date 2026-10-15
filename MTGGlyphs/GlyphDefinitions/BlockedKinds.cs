namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"creatures with power 2 or greater", "white creatures with power 2 or greater": the creatures a "can't block" restriction is about.</summary>
/// <exampledoc>Brassclaw Orcs</exampledoc>
/// <examplecapture>creatures with power 2 or greater</examplecapture>
[Dependent]
public class BlockedKinds : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural(Prop(Kind)), Prop(With)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public PermanentKind Kind { get; set; }
    public WithQualifier With { get; set; }
}
