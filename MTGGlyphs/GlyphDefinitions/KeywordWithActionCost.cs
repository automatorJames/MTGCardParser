namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword whose cost is an action rather than mana, written "keyword—cost": "kicker—sacrifice a land", "buyback—discard two cards".</summary>
/// <exampledoc>Falling Timber</exampledoc>
/// <examplecapture>kicker—sacrifice a land</examplecapture>
[Dependent]
public class KeywordWithActionCost : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Keyword), "—", Prop(Cost)];

    public Keyword Keyword { get; set; }
    public Cost Cost { get; set; }
}
