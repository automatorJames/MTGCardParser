namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Base power and toughness set by a measure: "base power and base toughness each equal to its mana value", "base power and toughness each equal to x plus 1".</summary>
/// <exampledoc>Opalescence</exampledoc>
/// <examplecapture>base power and base toughness each equal to its mana value</examplecapture>
[Dependent]
public class BaseStatsEqualTo : Glyph
{
    public override Nib[] Nibs => ["base power and", Opt("base"), "toughness", Opt("each"), Prop(Amount)];

    public EqualTo Amount { get; set; }
}
