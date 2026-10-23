namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that permanent", "that artifact token": a permanent named earlier, by a word ThatCard doesn't read.</summary>
/// <exampledoc>Kinnan, Bonder Prodigy</exampledoc>
/// <examplecapture>that permanent</examplecapture>
[Dependent]
[RegexPattern("that (?:permanent|artifact token)")]
public class ThatPermanentWord : Glyph;
