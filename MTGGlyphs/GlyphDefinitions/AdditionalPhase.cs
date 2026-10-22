namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An extra combat (or main) phase: "after this phase, there is an additional combat phase", "after this main phase, there is an additional combat phase followed by an additional main phase", "there's an additional combat phase after this phase", "after this main phase, there are two additional combat phases". Also the second of two actions ("untap all creatures you control and after this phase, there is …") or the effect of a condition.</summary>
/// <exampledoc>Fury of the Horde</exampledoc>
/// <examplecapture>after this main phase, there is an additional combat phase followed by an additional main phase</examplecapture>
public class AdditionalPhase : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Before), Alt("there is", "there's", "there are"), Prop(Count), Plural(Prop(Phase)), Prop(After), Prop(FollowedByMainPhase)];

    [Optional]
    public PhaseAnchor? Before { get; set; }
    public AdditionalCount Count { get; set; }
    public Phase Phase { get; set; }
    [Optional]
    [RegexPattern("after this phase")]
    public bool? After { get; set; }
    [RegexPattern("followed by an additional main phase")]
    public bool FollowedByMainPhase { get; set; }
}
