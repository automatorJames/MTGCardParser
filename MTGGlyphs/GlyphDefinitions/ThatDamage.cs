namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The damage a replacement condition just named, doubled or adjusted: "double that damage", "that much damage plus 1", "that much damage minus 1".</summary>
/// <exampledoc>Dictate of the Twin Gods</exampledoc>
/// <examplecapture>double that damage</examplecapture>
[Dependent]
public class ThatDamage : Glyph
{
    public override Nib[] Nibs => [Prop(Factor), Alt("that damage", "that much damage"), Prop(Adjustment)];

    [Optional]
    public AmountFactor? Factor { get; set; }
    [Optional]
    public AmountAdjustment Adjustment { get; set; }
}
