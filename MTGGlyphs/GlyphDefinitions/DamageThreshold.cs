namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount of damage bounded rather than fixed, as replacement and prevention conditions name it: "3 or more damage", "3 or less damage", "1 or more damage".</summary>
/// <exampledoc>Divine Presence</exampledoc>
/// <examplecapture>4 or more damage</examplecapture>
[Dependent]
public class DamageThreshold : Glyph
{
    public override Nib[] Nibs => [Prop(Count), "or", Prop(Bound), "damage"];

    public Quantity Count { get; set; }
    public CountBound Bound { get; set; }
}
