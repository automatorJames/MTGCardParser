namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Ending the turn, the cleanup it skips to: "discard down to your maximum hand size", "the player whose turn it is discards down to their maximum hand size".</summary>
/// <exampledoc>Glorious End</exampledoc>
/// <examplecapture>the player whose turn it is discards down to their maximum hand size</examplecapture>
public class DiscardToHandSize : Glyph
{
    public override Nib[] Nibs => [Prop(ActivePlayer), Alt("discard", "discards"), "down to", Alt("your", "their"), "maximum hand size"];

    [RegexPattern("the player whose turn it is")]
    public bool ActivePlayer { get; set; }
}
