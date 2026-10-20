namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's life total against a threshold: "a player has 13 or less life", "an opponent has 10 or less life", "you have 25 or more life".</summary>
/// <exampledoc>Murky Sewer</exampledoc>
/// <examplecapture>a player has 13 or less life</examplecapture>
[Dependent]
public class LifeThreshold : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Verb), Prop(Life), "or", Prop(Bound), "life"];

    public WhichPlayer Player { get; set; }
    public HaveVerb Verb { get; set; }
    [RegexPattern(@"\d+")]
    public int Life { get; set; }
    public LifeBound Bound { get; set; }
}
