namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The parenthetical count after energy: "(two energy counters)", "(an energy counter)", "(energy counters)".</summary>
/// <exampledoc>Aether Chaser</exampledoc>
/// <examplecapture>(two energy counters)</examplecapture>
[Dependent]
public class EnergyNote : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["(", Prop(Count), Plural("energy counter"), ")"];

    [Optional]
    public CountWord Count { get; set; }
}
