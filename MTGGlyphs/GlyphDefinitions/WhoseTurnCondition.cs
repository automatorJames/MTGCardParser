namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose turn it is, as a condition: "it's your turn", "it's not your turn", "it's an opponent's turn", "it's not their turn".</summary>
/// <exampledoc>Fated Conflagration</exampledoc>
/// <examplecapture>it's your turn</examplecapture>
[Dependent]
public class WhoseTurnCondition : Glyph
{
    public override Nib[] Nibs => ["it's", Prop(Not), Prop(Whose), "turn"];

    [RegexPattern("not")]
    public bool Not { get; set; }
    public Whose Whose { get; set; }
}
