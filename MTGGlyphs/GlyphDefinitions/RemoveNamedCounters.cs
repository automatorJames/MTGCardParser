namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Removing counters named earlier in the line: "remove them", "remove all of them from it", "remove them all" - after a condition counted them ("if it has three or more ritual counters on it, remove them and transform it").</summary>
/// <exampledoc>Heirloom Mirror // Inherited Fiend</exampledoc>
/// <examplecapture>remove them</examplecapture>
public class RemoveNamedCounters : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["remove", Prop(Counters), Prop(Place)];

    public NamedCounters Counters { get; set; }
    [Optional]
    public CounterPlace Place { get; set; }
}
