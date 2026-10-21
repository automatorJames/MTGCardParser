namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A group of permanents that something taps or untaps, destroys or exiles: counted targets ("x target creatures"), all of a kind ("all other creatures", "all creatures you control"), a number of a kind ("up to seven lands"), or a group named earlier ("them").</summary>
/// <exampledoc>Sky Hussar</exampledoc>
/// <examplecapture>all creatures you control</examplecapture>
[Dependent]
public class PermanentGroup : GlyphOneOf
{
    public CountedTargets Counted { get; set; }
    public AllPermanents All { get; set; }
    public UpToPermanents UpTo { get; set; }
    public Them Them { get; set; }
}
