namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"this creature" or "this permanent", in reminder text: the permanent that has the ability being explained.</summary>
/// <exampledoc>Living Wall</exampledoc>
/// <examplecapture>this creature</examplecapture>
[Dependent]
[RegexPattern("this (creature|permanent)")]
[Referent]
public class ThisCreature : Glyph;
