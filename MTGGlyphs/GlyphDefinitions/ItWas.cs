namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it was": a back-reference to the card just named, as it last was ("if it was a creature card", after it's exiled from a graveyard).</summary>
/// <exampledoc>Scavenging Ooze</exampledoc>
/// <examplecapture>it was</examplecapture>
[Dependent]
[RegexPattern("it was")]
public class ItWas : BackReference;
