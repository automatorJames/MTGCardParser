namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"[+1]", "[−3]", "[0]", "[−x]": the loyalty counters added or removed to activate a loyalty ability.</summary>
/// <exampledoc>Ob Nixilis Reignited</exampledoc>
/// <examplecapture>[−3]</examplecapture>
[Dependent]
public class LoyaltyCost : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["[", Prop(Sign), Prop(Amount), "]"];

    [Optional]
    public LoyaltySign? Sign { get; set; }
    public Quantity Amount { get; set; }
}
