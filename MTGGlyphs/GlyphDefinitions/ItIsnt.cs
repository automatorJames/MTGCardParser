namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it isn't" / "it is not": a back-reference to the card just named, as the subject of a negated "is" ("if it isn't a land card").</summary>
/// <exampledoc>Fa'adiyah Seer</exampledoc>
/// <examplecapture>it isn't</examplecapture>
[Dependent]
[RegexPattern("it isn't|it is not")]
public class ItIsnt : BackReference;
