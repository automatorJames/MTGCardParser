namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"end the turn": the Time Stop effect, whose reminder text follows.</summary>
/// <exampledoc>Glorious End</exampledoc>
/// <examplecapture>end the turn</examplecapture>
[RegexPattern("end the turn")]
public class EndTheTurn : Glyph, IPredicate;
