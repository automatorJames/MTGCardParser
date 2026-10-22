namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you or a permanent you control", as what damage is dealt to.</summary>
/// <exampledoc>Flameblade Angel</exampledoc>
/// <examplecapture>you or a permanent you control</examplecapture>
[Dependent]
[RegexPattern("you or a permanent you control")]
public class YouOrAPermanentYouControl : Glyph;
