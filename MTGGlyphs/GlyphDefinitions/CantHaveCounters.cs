namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent counters can't be put on: "can't have counters put on it", "can't have -1/-1 counters put on them".</summary>
/// <exampledoc>Melira's Keepers</exampledoc>
/// <examplecapture>can't have counters put on it</examplecapture>
[Dependent]
public class CantHaveCounters : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't have", Prop(Kind), "counters put on", Prop(Bearer)];

    [Optional]
    public CounterType? Kind { get; set; }
    public OneOf<It, Them> Bearer { get; set; }
}
