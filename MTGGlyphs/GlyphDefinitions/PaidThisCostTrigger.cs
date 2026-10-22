namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The reflexive trigger after a repeatable payment: "when you pay this cost one or more times, put that many valor counters on this creature" - its effect usually counts the payments with "that many".</summary>
/// <exampledoc>Intrepid Adversary</exampledoc>
/// <examplecapture>when you pay this cost one or more times, put that many valor counters on this creature</examplecapture>
public class PaidThisCostTrigger : Glyph
{
    public override Nib[] Nibs => ["when you pay this cost one or more times,", Prop(Effect)];

    public DynamicGlyph Effect { get; set; }
}
