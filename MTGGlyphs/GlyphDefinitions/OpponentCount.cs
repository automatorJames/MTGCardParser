namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the number of opponents you have": how many opponents the player has, a count used in multiplayer-aware effects.</summary>
/// <exampledoc>Chittering Witch</exampledoc>
/// <examplecapture>the number of opponents you have</examplecapture>
[Dependent]
[RegexPattern("the number of opponents you have")]
public class OpponentCount : Glyph;
