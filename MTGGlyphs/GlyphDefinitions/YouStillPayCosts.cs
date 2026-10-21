namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A reminder that casting a card from an unusual zone still means paying for it: "you still pay its costs".</summary>
/// <exampledoc>Emry, Lurker of the Loch</exampledoc>
/// <examplecapture>you still pay its costs</examplecapture>
public class YouStillPayCosts : Glyph
{
    public override Nib[] Nibs => ["you still pay its costs"];
}
