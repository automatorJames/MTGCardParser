namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cumulative upkeep with its per-counter cost: "cumulative upkeep {1}", "cumulative upkeep—pay 1 life", or a choice of mana, "cumulative upkeep {g} or {w}".</summary>
/// <exampledoc>Energy Storm</exampledoc>
/// <examplecapture>cumulative upkeep {1}</examplecapture>
[Dependent]
public class CumulativeUpkeep : Glyph
{
    public override Nib[] Nibs => [Pattern("cumulative upkeep(—pay)?"), Prop(Cost), Prop(OrMana)];

    public Cost Cost { get; set; }
    [Optional]
    public OrManaCost OrMana { get; set; }
}
