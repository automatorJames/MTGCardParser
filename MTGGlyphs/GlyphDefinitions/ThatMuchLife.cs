namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that much life", "twice that much life", "that much life plus 1": an amount of life equal to a number just mentioned, possibly doubled or adjusted - as replacement effects say it.</summary>
/// <exampledoc>Basilica Guards</exampledoc>
/// <examplecapture>that much life</examplecapture>
[Dependent]
public class ThatMuchLife : Glyph
{
    public override Nib[] Nibs => [Prop(Factor), "that much life", Prop(Adjustment)];

    [Optional]
    public AmountFactor? Factor { get; set; }
    [Optional]
    public AmountAdjustment Adjustment { get; set; }
}
