namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"this creature", "this permanent" or "this card" in reminder text: the object that has the ability being explained.</summary>
/// <exampledoc>Living Wall</exampledoc>
/// <examplecapture>this creature</examplecapture>
[Dependent]
[RegexPattern("this (creature|permanent|card)")]
[Referent]
public class ThisCreature : Glyph;
