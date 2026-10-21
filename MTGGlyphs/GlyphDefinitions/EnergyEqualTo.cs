namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"an amount of {e} equal to the greatest power among creatures you control".</summary>
/// <exampledoc>Peema Aether-Seer</exampledoc>
/// <examplecapture>an amount of {e} (energy counters) equal to the greatest power among creatures you control</examplecapture>
[Dependent]
public class EnergyEqualTo : Glyph
{
    public override Nib[] Nibs => ["an amount of {e}", Prop(Note), Prop(EqualTo)];

    [Optional]
    public EnergyNote Note { get; set; }
    public EqualTo EqualTo { get; set; }
}
