namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A line of nothing but keyword abilities: "flying", "flying, trample", "protection from red". Keyword actions are lines like this too ("scry 2", "investigate"), so it's also a predicate: "you may clash with an opponent", "you may investigate".</summary>
/// <exampledoc>Lord of the Pit</exampledoc>
/// <examplecapture>flying, trample</examplecapture>
public class CardAbilityLine : CompoundOf<KeywordAbility>, IPredicate;