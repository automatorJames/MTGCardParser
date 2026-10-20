namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"by walls", "by artifact creatures", "by creatures with flying or reach", "by creatures with power 3 or greater", "by more than one creature", "by three or more creatures": which permanents, or how many, a combat restriction is about.</summary>
/// <exampledoc>Juggernaut</exampledoc>
/// <examplecapture>by walls</examplecapture>
[Dependent]
public class CombatantsBy : Glyph
{
    public override Nib[] Nibs => ["by", Prop(Count), Prop(Qualifiers), Plural(Prop(Kind)), Prop(With)];

    [Optional]
    public OneOf<MoreThanCount, AtLeastCount> Count { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public PermanentKind Kind { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
