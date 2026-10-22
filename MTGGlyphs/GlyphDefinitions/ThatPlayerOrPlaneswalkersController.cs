namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that player or that planeswalker's controller": the player hit by an earlier "target player or planeswalker", or the planeswalker's controller.</summary>
/// <exampledoc>Flame Wave</exampledoc>
/// <examplecapture>that player or that planeswalker's controller</examplecapture>
[Dependent]
[RegexPattern("that player or that planeswalker's controller")]
public class ThatPlayerOrPlaneswalkersController : Glyph;
