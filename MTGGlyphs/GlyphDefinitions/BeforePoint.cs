namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"before blockers are declared", "before the combat damage step": up to a point in the turn.</summary>
/// <exampledoc>Rapid Fire</exampledoc>
/// <examplecapture>before blockers are declared</examplecapture>
[Dependent]
public class BeforePoint : Glyph
{
    public override Nib[] Nibs => ["before", Prop(Point)];

    public OneOf<CombatMilestone?, PlayersPhase> Point { get; set; }
}
