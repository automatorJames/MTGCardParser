namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it was": a back-reference to an object that has left the battlefield, as a "dies" trigger's condition has it ("if it was a creature", "if it was a human").</summary>
/// <exampledoc>Avacyn's Collar</exampledoc>
/// <examplecapture>it was</examplecapture>
[Dependent]
[RegexPattern("it was")]
public class ItWas : BackReference;
