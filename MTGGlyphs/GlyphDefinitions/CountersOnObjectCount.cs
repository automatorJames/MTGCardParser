namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the number of charge counters on this artifact", "the number of rust counters on it": a count of one kind of counter on this object or one named earlier - small enough to sit inside a with-qualifier.</summary>
/// <exampledoc>Aether Vial</exampledoc>
/// <examplecapture>the number of charge counters on this artifact</examplecapture>
[Dependent]
public class CountersOnObjectCount : Glyph
{
    public override Nib[] Nibs => ["the number of", Prop(Kind), "counters on", Prop(Holder)];

    public CounterType Kind { get; set; }
    public OneOf<ThisCreature, It> Holder { get; set; }
}
