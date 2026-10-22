namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"before blockers are declared", "before the combat damage step", "after blockers are declared", "after combat", "after their upkeep step": up to, or from, a point in the turn.</summary>
/// <exampledoc>Rapid Fire</exampledoc>
/// <examplecapture>before blockers are declared</examplecapture>
[Dependent]
public class BeforePoint : Glyph
{
    public override Nib[] Nibs => [Prop(Relation), Prop(Point)];

    public PointRelation Relation { get; set; }
    public OneOf<CombatMilestone?, PlayersPhase> Point { get; set; }
}
