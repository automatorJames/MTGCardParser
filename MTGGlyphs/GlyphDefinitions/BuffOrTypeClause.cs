namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One clause in a list of what a permanent gets, has or is: a buff ("gets +2/+2", "has flying") or a type it gains ("is an angel in addition to its other types").</summary>
/// <exampledoc>Angelic Destiny</exampledoc>
/// <examplecapture>is an angel in addition to its other types</examplecapture>
[Dependent]
public class BuffOrTypeClause : GlyphOneOf
{
    public GainedOrLostBuff GainedOrLostBuff { get; set; }
    public IsOfType IsOfType { get; set; }
}
