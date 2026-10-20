namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"this creature", "this land", "this artifact", "this permanent", "this card" and the like: the object that has the ability, as modern Oracle text names it instead of the card's name. Bare "this" too, as level up's reminder has it ("put a level counter on this").</summary>
/// <exampledoc>Living Wall</exampledoc>
/// <examplecapture>this creature</examplecapture>
[Dependent]
[RegexPattern("this( (creature|permanent|card|land|artifact|enchantment|equipment|aura|vehicle|saga|planeswalker|token|battle|spacecraft|planet))?")]
[Referent]
public class ThisCreature : Glyph;
