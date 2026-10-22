namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the number of charge counters on this artifact", "the number of oil counters on {this}", "the number of verse counters on yisan", "the number of rust counters on it": a count of one kind of counter on this object (by name, short name or "this …") or one named earlier - small enough to sit inside a with-qualifier.</summary>
/// <exampledoc>Aether Vial</exampledoc>
/// <examplecapture>the number of charge counters on this artifact</examplecapture>
[Dependent]
public class CountersOnObjectCount : Glyph
{
    public override Nib[] Nibs => ["the number of", Prop(Kind), "counters on", Prop(Holder)];

    public CounterType Kind { get; set; }
    public OneOf<OneOf<This, ThisCreature>, It, ShortName> Holder { get; set; }
}
