namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"by walls", "by artifact creatures", "by creatures with flying or reach": which permanents a combat restriction is about.</summary>
/// <exampledoc>Juggernaut</exampledoc>
/// <examplecapture>by walls</examplecapture>
[Dependent]
public class CombatantsBy : Glyph
{
    public override Nib[] Nibs => ["by", Prop(Qualifiers), Plural(Prop(Kind)), Prop(With)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public PermanentKind Kind { get; set; }
    [Optional]
    public WithKeywords With { get; set; }
}
