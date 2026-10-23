namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword ability's cost that an alternative cost replaces: "the echo cost for permanents you control", "cycling costs", "the equip cost of the first equip ability you activate each turn".</summary>
/// <exampledoc>Thick-Skinned Goblin</exampledoc>
/// <examplecapture>the echo cost for permanents you control</examplecapture>
[Dependent]
public class ReplacedKeywordCost : Glyph
{
    public override Nib[] Nibs => [Opt("the"), Prop(Keyword), Plural("cost"), Prop(Scope)];

    public Keyword Keyword { get; set; }
    [Optional]
    public OneOf<PermanentsYouControlScope, FirstAbilityEachTurn> Scope { get; set; }
}
