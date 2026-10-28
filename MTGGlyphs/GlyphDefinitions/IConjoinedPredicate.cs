namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Marker for a predicate that can be the second half of an "and" pair - what <see cref="PredicatePair"/>'s second slot resolves to ("gets +2/+0 until end of turn and deals 1 damage to you"). Kept narrower than <see cref="IPredicate"/> because the first slot ends at the last "and" of the clause, so only predicates that read well after one belong here.</summary>
public interface IConjoinedPredicate
{
}
