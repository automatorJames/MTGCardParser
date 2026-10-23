namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A space-joined run of aspects describing one permanent: "basic mountain", "legendary creature", "white artifact".</summary>
/// <exampledoc>Goblin Caves</exampledoc>
/// <examplecapture>basic mountain</examplecapture>
[Dependent]
[JoinedBy(Joiner.Space)]
public class AspectRun : CompoundOf<CardAspect>;
