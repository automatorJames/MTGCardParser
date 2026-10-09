namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A line of nothing but keyword abilities: "flying", "flying, trample", "protection from red".</summary>
/// <exampledoc>Lord of the Pit</exampledoc>
/// <examplecapture>flying, trample</examplecapture>
public class CardAbilityLine : CompoundOf<KeywordAbility>;