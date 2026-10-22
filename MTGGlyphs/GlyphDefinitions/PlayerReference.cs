namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player named earlier in the line: "that player", the controller of a permanent named earlier, "that creature's controller", or either of those after "target player or planeswalker" ("that player or that planeswalker's controller").</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>that creature's controller</examplecapture>
[Dependent]
public class PlayerReference : GlyphOneOf
{
    public ThatPlayerOrPlaneswalkersController PlayerOrPlaneswalkersController { get; set; }
    public ThatCardsController ThatCardsController { get; set; }
    public ThatPlayer ThatPlayer { get; set; }
}
