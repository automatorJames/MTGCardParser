namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One option in a choice of buffs: a keyword, a keyword with a number, or protection.</summary>
/// <exampledoc>Jodah's Avenger</exampledoc>
/// <examplecapture>protection from red</examplecapture>
[Dependent]
public class ChoosableBuff : GlyphOneOf
{
    public Protection Protection { get; set; }
    public KeywordWithAmount KeywordWithAmount { get; set; }
    public Keyword? Keyword { get; set; }
}
