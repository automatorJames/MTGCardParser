namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A predicate stating the subject has no color, as devoid's reminder text: "this card has no color".</summary>
/// <exampledoc>Abstruse Interference</exampledoc>
/// <examplecapture>has no color</examplecapture>
[Dependent]
public class HasNoColor : Glyph, IPredicate;
