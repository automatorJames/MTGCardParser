namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"flip a coin": a random binary outcome, read by a following "if you win the flip" / "if you lose the flip". Also a predicate, so "you may flip a coin" resolves.</summary>
/// <exampledoc>Bottle of Suleiman</exampledoc>
/// <examplecapture>flip a coin</examplecapture>
public class FlipACoin : Glyph, IConjoinedPredicate, IPredicate;
