namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Getting energy counters, optionally with its parenthetical count: "you get {e}{e} (two energy counters)".</summary>
/// <exampledoc>Aether Chaser</exampledoc>
/// <examplecapture>you get {e}{e} (two energy counters)</examplecapture>
public class GetEnergy : Glyph
{
    public override Nib[] Nibs => ["you get", Pattern(@"(\{e\})+"), Prop(Note)];

    [Optional]
    public EnergyNote Note { get; set; }
}
