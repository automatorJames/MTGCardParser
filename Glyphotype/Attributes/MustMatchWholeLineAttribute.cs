namespace Glyphotype.Attributes;

/// <summary>
/// Declares that a top-level <see cref="Glyph"/> type takes an entire line or nothing: it is only ever a
/// candidate at the tokenization scope's own start, and its match must run to the scope's end. Nothing
/// else can share the pass - no other token before it, none after it.
/// <para>
/// The strictest of the three span rules, and the only one that holds regardless of
/// <see cref="GlobalSettings.AllowPartialSegmentMatches"/>: it predates that setting and is stricter than
/// anything the setting imposes, so it simply overrides the question. Contrast
/// <see cref="AllowPartialSegmentMatchAttribute"/>, which sits at the opposite end of the same axis (how
/// much must a match cover?) and is therefore mutually exclusive with this - see
/// <see cref="Glyph.ValidateStructure"/>. A type wanting the middle ground, "a whole number of clauses
/// rather than the whole line", declares a bare <c>"."</c> nib instead (see
/// <see cref="RegexGraph.SpansClauses"/>), which is about crossing periods rather than about coverage.
/// </para>
/// <para>
/// Note this grants scope, not matching power: a whole-line type still has to actually match every period
/// it crosses, so one covering a multi-clause line needs those periods in its own pattern.
/// </para>
/// <para>
/// Mutually exclusive with <see cref="DependentAttribute"/>: a dependent only ever matches nested inside a
/// parent's pattern, so it can never independently be the whole line.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MustMatchWholeLineAttribute : Attribute
{
}

