namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two costs paid together, joined by "and" rather than a comma: "remove three quest counters from this enchantment and sacrifice it", "remove an eon counter from this land and return it to its owner's hand" - the counters come off, then the permanent goes.</summary>
/// <exampledoc>Ior Ruin Expedition</exampledoc>
/// <examplecapture>remove three quest counters from this enchantment and sacrifice it</examplecapture>
[Dependent]
public class CountersAndSacrificeCost : Glyph
{
    public override Nib[] Nibs => [Prop(Remove), "and", Prop(SacrificeOrReturn)];

    public PlaceCounters Remove { get; set; }
    public OneOf<Sacrifice, ReturnCard> SacrificeOrReturn { get; set; }
}
