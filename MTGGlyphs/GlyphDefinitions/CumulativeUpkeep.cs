namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cumulative upkeep with its per-counter cost: "cumulative upkeep {1}", "cumulative upkeep—pay 1 life".</summary>
/// <exampledoc>Energy Storm</exampledoc>
/// <examplecapture>cumulative upkeep {1}</examplecapture>
[Dependent]
public class CumulativeUpkeep : Glyph
{
    public override Nib[] Nibs => [Pattern("cumulative upkeep(—pay)?"), Prop(Cost)];

    public Cost Cost { get; set; }
}
