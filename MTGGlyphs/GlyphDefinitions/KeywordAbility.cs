namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One keyword ability: a plain keyword ("flying") or one with a parameter ("protection from red").</summary>
/// <exampledoc>Repentant Blacksmith</exampledoc>
/// <examplecapture>protection from red</examplecapture>
[Dependent]
public class KeywordAbility : GlyphOneOf
{
    public Protection Protection { get; set; }
    public Keyword? Keyword { get; set; }
}
