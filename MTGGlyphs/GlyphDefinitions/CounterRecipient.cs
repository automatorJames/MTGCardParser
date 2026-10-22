namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One or every permanent of a kind receiving or losing counters: "each creature you control", "each other creature", "all creatures", "a creature you control" (one of your choice), several targets each getting them in full ("each of up to two target creatures"), each of a group named earlier ("each of them"), a counted target ("up to one target creature"), a single permanent, or, last, a legend by its short name ("put a +1/+1 counter on zurgo").</summary>
/// <exampledoc>Abzan Ascendancy</exampledoc>
/// <examplecapture>each creature you control</examplecapture>
[Dependent]
public class CounterRecipient : GlyphOneOf
{
    public EachOfTargets EachOfTargets { get; set; }
    public EachOfThem EachOfThem { get; set; }
    public EachPermanents EachPermanents { get; set; }
    public AllPermanents AllPermanents { get; set; }
    public CountedTargets CountedTargets { get; set; }
    public PermanentPhrase PermanentPhrase { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
    public ShortName ShortName { get; set; }
}
