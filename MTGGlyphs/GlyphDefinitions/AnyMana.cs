namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"mana", unqualified: any mana, of whatever type, as in "for each mana spent to cast it".</summary>
/// <exampledoc>Verazol, the Split Current</exampledoc>
/// <examplecapture>mana</examplecapture>
[Dependent]
[RegexPattern("mana")]
public class AnyMana : Glyph;
