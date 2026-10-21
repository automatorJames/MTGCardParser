namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount of energy: symbols ("{e}{e}"), a counted number ("six {e}", "that many {e}"), or "an amount of {e} equal to …".</summary>
/// <exampledoc>Aethertide Whale</exampledoc>
/// <examplecapture>six {e}</examplecapture>
[Dependent]
public class EnergyAmount : GlyphOneOf
{
    public EnergySymbols Symbols { get; set; }
    public CountedEnergy Counted { get; set; }
    public EnergyEqualTo EqualTo { get; set; }
}
