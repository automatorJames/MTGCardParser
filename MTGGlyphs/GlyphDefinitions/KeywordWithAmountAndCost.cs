namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword with both a number and a mana cost, written "N—cost": "suspend 3—{w}", "suspend x—{x}{3}{u}", "reinforce 1—{1}{w}".</summary>
/// <exampledoc>Rift Bolt</exampledoc>
/// <examplecapture>suspend 1—{r}</examplecapture>
[Dependent]
public class KeywordWithAmountAndCost : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Keyword), " ", Prop(Amount), "—", Prop(Cost)];

    public Keyword Keyword { get; set; }
    public Quantity Amount { get; set; }
    public ManaCost Cost { get; set; }
}
