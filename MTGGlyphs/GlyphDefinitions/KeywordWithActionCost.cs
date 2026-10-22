namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword whose cost is an action rather than mana, or several costs, written "keyword—cost": "kicker—sacrifice a land", "buyback—discard two cards", "flashback—{1}{g}, pay 3 life", "ward—{2}, pay 2 life".</summary>
/// <exampledoc>Falling Timber</exampledoc>
/// <examplecapture>kicker—sacrifice a land</examplecapture>
[Dependent]
public class KeywordWithActionCost : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Keyword), "—", Prop(Costs)];

    public Keyword Keyword { get; set; }
    public CompoundOf<Cost> Costs { get; set; }
}
