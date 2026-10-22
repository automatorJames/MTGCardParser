namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An ability chosen earlier in the line or ability, referred back to: "that ability" (after "choose flying, first strike, or trample").</summary>
/// <exampledoc>Angelic Skirmisher</exampledoc>
/// <examplecapture>that ability</examplecapture>
[Dependent]
[RegexPattern("that ability")]
public class ChosenAbility : Glyph;
