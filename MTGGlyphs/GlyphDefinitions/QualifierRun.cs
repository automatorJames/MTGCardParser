namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Qualifier words in a row, space-joined: "nonland permanent", "legendary creature".</summary>
/// <exampledoc>Black Vise</exampledoc>
/// <examplecapture>nonland permanent</examplecapture>
[JoinedBy(Joiner.Space)]
public class QualifierRun : CompoundOf<TargetQualifier>;
