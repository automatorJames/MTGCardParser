namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Stats an animated permanent gets from a number: "with power and toughness each equal to its mana value", "with base power and toughness each equal to x plus 1".</summary>
/// <exampledoc>Karn's Touch</exampledoc>
/// <examplecapture>with power and toughness each equal to its mana value</examplecapture>
[Dependent]
public class WithStatsEqualTo : Glyph
{
    public override Nib[] Nibs => ["with", Opt("base"), "power and toughness each equal to", Prop(Amount)];

    public Amount Amount { get; set; }
}
