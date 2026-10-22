namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The floor on cost reductions for activated abilities (Training Grounds, Heartstone): "this effect can't reduce the mana in that cost to less than one mana".</summary>
/// <exampledoc>Training Grounds</exampledoc>
/// <examplecapture>this effect can't reduce the mana in that cost to less than one mana</examplecapture>
[RegexPattern("this effect can't reduce the mana in that cost to less than one mana")]
public class CostReductionFloor : Glyph;
