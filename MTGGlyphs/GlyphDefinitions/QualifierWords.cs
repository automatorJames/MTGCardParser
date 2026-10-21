namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A run of qualifier words before a noun, any of them a choice: "dragon permanent", "blue or red creature", "nonland", "instant or sorcery".</summary>
/// <exampledoc>Crypt Angel</exampledoc>
/// <examplecapture>blue or red creature</examplecapture>
[JoinedBy(Joiner.Space)]
public class QualifierWords : CompoundOf<OneOf<ManyOf<TargetQualifier>, TargetQualifier>>;
