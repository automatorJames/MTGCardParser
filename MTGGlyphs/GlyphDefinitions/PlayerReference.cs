namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player named earlier in the line: "that player", or the controller of a permanent named earlier, "that creature's controller".</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>that creature's controller</examplecapture>
[Dependent]
public class PlayerReference : GlyphOneOf
{
    public ThatCardsController ThatCardsController { get; set; }
    public ThatPlayer ThatPlayer { get; set; }
}
