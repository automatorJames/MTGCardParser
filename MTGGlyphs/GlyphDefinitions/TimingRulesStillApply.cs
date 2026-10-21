namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A reminder that permission to cast a card doesn't change when it may be cast: "timing rules still apply", often after "(you still pay its costs".</summary>
/// <exampledoc>Emry, Lurker of the Loch</exampledoc>
/// <examplecapture>timing rules still apply</examplecapture>
public class TimingRulesStillApply : Glyph
{
    public override Nib[] Nibs => ["timing rules still apply"];
}
