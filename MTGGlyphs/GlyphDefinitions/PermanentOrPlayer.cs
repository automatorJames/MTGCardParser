namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a permanent or player": any one thing damage can be dealt to, as replacement effects on damage name it.</summary>
/// <exampledoc>Dictate of the Twin Gods</exampledoc>
/// <examplecapture>a permanent or player</examplecapture>
[Dependent]
[RegexPattern("a permanent or player")]
public class PermanentOrPlayer : Glyph;
