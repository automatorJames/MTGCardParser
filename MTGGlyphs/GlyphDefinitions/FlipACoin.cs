namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"flip a coin", "flips a coin": a random binary outcome, read by a following "if you win the flip" / "if you lose the flip". Also a predicate, so "you may flip a coin" and "each player flips a coin" resolve.</summary>
/// <exampledoc>Bottle of Suleiman</exampledoc>
/// <examplecapture>flip a coin</examplecapture>
[RegexPattern("flips? a coin")]
public class FlipACoin : Glyph, IConjoinedPredicate, IPredicate;
