namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Power and/or toughness set equal to something: "with power and toughness each equal to its mana value".</summary>
/// <exampledoc>Animate Artifact</exampledoc>
/// <examplecapture>with power and toughness each equal to its mana value</examplecapture>
[Dependent]
public class WithPowerToughnessEqual : Glyph
{
    public override Nib[] Nibs => ["with", Prop(PowerAndOrToughness), Opt("each"), "equal to", Prop(EquivalentToMeasurement)];

    public PowerAndOrToughness PowerAndOrToughness { get; set; }
    public EquivalentToMeasurement EquivalentToMeasurement { get; set; }
}
