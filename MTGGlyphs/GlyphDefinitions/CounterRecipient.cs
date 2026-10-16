namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One or every permanent of a kind receiving or losing counters: "each creature you control", "each other creature", "all creatures", or a single permanent.</summary>
/// <exampledoc>Abzan Ascendancy</exampledoc>
/// <examplecapture>each creature you control</examplecapture>
[Dependent]
public class CounterRecipient : GlyphOneOf
{
    public EachPermanents EachPermanents { get; set; }
    public AllPermanents AllPermanents { get; set; }
    public PermanentPhrase PermanentPhrase { get; set; }
}
