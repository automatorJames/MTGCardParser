namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"each kind of": every counter type on an object at once, as in "double the number of each kind of counter on target permanent".</summary>
/// <exampledoc>Gilder Bairn</exampledoc>
/// <examplecapture>each kind of</examplecapture>
[Dependent]
[RegexPattern("each kind of")]
public class EachKindOfCounter : Glyph;
