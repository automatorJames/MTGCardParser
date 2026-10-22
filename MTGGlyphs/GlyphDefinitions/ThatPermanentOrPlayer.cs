namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that permanent or player": the permanent or player a damage replacement's condition named, as the recipient of its replacement.</summary>
/// <exampledoc>Dictate of the Twin Gods</exampledoc>
/// <examplecapture>that permanent or player</examplecapture>
[Dependent]
[RegexPattern("that (permanent or player|player or permanent)")]
public class ThatPermanentOrPlayer : Glyph;
