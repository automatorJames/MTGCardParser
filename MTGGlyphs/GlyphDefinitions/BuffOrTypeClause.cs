namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One clause in a list of what a permanent gets, has or is: a buff ("gets +2/+2", "has flying"), a type it gains ("is an angel in addition to its other types", "is every creature type"), or a color it is ("is black").</summary>
/// <exampledoc>Angelic Destiny</exampledoc>
/// <examplecapture>is an angel in addition to its other types</examplecapture>
[Dependent]
public class BuffOrTypeClause : GlyphOneOf
{
    public GainedOrLostBuff GainedOrLostBuff { get; set; }
    public IsOfType IsOfType { get; set; }
    public IsColor IsColor { get; set; }
    public IsEveryCreatureType IsEveryCreatureType { get; set; }
}
