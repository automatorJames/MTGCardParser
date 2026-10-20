namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that's" / "that is": a back-reference to the object just named, with the verb run into it, as backup's reminder text has it.</summary>
/// <exampledoc>Saiba Cryptomancer</exampledoc>
/// <examplecapture>that's</examplecapture>
[Dependent]
[RegexPattern("that's|that is")]
public class ThatIs : BackReference;
