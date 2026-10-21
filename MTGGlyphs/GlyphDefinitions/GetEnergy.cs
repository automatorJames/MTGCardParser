namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Getting energy counters: "you get {e}{e} (two energy counters)", "you get six {e} (energy counters)", "you get that many {e}", "you get an amount of {e} equal to …", optionally multiplied ("for each creature you control"). A predicate, so any subject and "then" sequences can take it.</summary>
/// <exampledoc>Aether Chaser</exampledoc>
/// <examplecapture>you get {e}{e} (two energy counters)</examplecapture>
public class GetEnergy : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["get", Prop(Amount), Prop(Note), Prop(Multiplier)];

    public EnergyAmount Amount { get; set; }
    [Optional]
    public EnergyNote Note { get; set; }
    [Optional]
    public ForEach Multiplier { get; set; }
}
