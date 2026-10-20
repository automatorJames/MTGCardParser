namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the following ability", "the following abilities": the abilities printed after backup's reminder, granted to the creature that got the counters.</summary>
/// <exampledoc>Saiba Cryptomancer</exampledoc>
/// <examplecapture>the following ability</examplecapture>
[Dependent]
[RegexPattern("the following abilit(y|ies)")]
public class FollowingAbilities : Glyph;
