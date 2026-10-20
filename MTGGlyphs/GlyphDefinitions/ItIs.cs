namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's" / "it is": a back-reference to the object just named, with the verb run into it.</summary>
/// <exampledoc>Call of the Wild</exampledoc>
/// <examplecapture>it's</examplecapture>
[Dependent]
[RegexPattern("it's|it is")]
public class ItIs : BackReference;
