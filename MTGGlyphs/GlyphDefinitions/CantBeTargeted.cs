namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Not targetable: hexproof's and shroud's reminder predicate, "can't be the target of spells or abilities", "can't be the targets of spells or abilities your opponents control", and its narrower forms: "can't be the target of blue or black spells", "can't be the target of white spells or abilities from white sources", "can't be the target of spells or abilities your opponents control this turn". Also the second of two predicates.</summary>
/// <exampledoc>Ascended Lawmage</exampledoc>
/// <examplecapture>can't be the target of spells or abilities your opponents control</examplecapture>
[Dependent]
public class CantBeTargeted : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't be the", Alt("target", "targets"), "of", Prop(By), Prop(Whose), Prop(Period)];

    public TargetingObjects By { get; set; }
    [Optional]
    public ControlledByWho Whose { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
}
