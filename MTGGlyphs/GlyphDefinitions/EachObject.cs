namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"each creature without flying", "each player", "each attacking creature", "each creature blocking or blocked by {this}": every object or player of a kind, one at a time.</summary>
/// <exampledoc>Earthquake</exampledoc>
/// <examplecapture>each creature without flying</examplecapture>
[Dependent]
public class EachObject : Glyph
{
    public override Nib[] Nibs => ["each", Prop(Qualifiers), Prop(Kind), Prop(With)];

    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public TargetKind Kind { get; set; }
    [Optional]
    public OneOf<WithKeywords, CombatRelation> With { get; set; }
}
