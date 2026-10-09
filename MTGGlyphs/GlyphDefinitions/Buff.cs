namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something a permanent can get or have: a power/toughness change ("+1/+1") or a keyword ("flying").</summary>
/// <exampledoc>Blessing</exampledoc>
/// <examplecapture>+1/+1</examplecapture>
[Dependent]
public class Buff : GlyphOneOf
{
    public PowerToughnessMod PowerToughnessMod { get; set; }
    public Keyword? Keyword { get; set; }
}
