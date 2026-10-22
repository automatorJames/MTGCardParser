namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cap on how many counters an ability may put on a permanent (the Clockwork creatures): "this ability can't cause the total number of +1/+0 counters on this creature to be greater than four".</summary>
/// <exampledoc>Clockwork Avian</exampledoc>
/// <examplecapture>this ability can't cause the total number of +1/+0 counters on this creature to be greater than four</examplecapture>
public class CounterCap : Glyph
{
    public override Nib[] Nibs => ["this ability can't cause the total number of", Prop(Kind), "counters on", Prop(Holder), "to be greater than", Prop(Limit)];

    public CounterType Kind { get; set; }
    public PermanentPhrase Holder { get; set; }
    public Quantity Limit { get; set; }
}
