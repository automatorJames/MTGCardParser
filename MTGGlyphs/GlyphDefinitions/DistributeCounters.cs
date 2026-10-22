namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Distributing counters among several objects: "distribute two +1/+1 counters among one or two target creatures", "distribute four +1/+1 counters among any number of creatures you control".</summary>
/// <exampledoc>Armament Corps</exampledoc>
/// <examplecapture>distribute two +1/+1 counters among one or two target creatures you control</examplecapture>
public class DistributeCounters : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["distribute", Prop(Counters), "among", Prop(Range), Prop(Targeted), Prop(Among)];

    public Counters Counters { get; set; }
    public DistributionRange Range { get; set; }
    [RegexPattern("target")]
    public bool Targeted { get; set; }
    public Permanents Among { get; set; }
}
