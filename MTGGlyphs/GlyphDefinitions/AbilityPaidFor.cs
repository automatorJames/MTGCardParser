namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"to activate this ability": the ability some mana paid for.</summary>
/// <exampledoc>Jetmir's Fixer</exampledoc>
/// <examplecapture>to activate this ability</examplecapture>
[Dependent]
[RegexPattern("to activate this ability")]
public class AbilityPaidFor : Glyph;
