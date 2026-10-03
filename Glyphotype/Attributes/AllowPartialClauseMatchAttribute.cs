namespace Glyphotype.Attributes;

/// <summary>
/// Declares that a top-level <see cref="Glyph"/> type may stop partway through a clause: it can begin
/// anywhere the cursor happens to be and end at any word boundary, rather than having to account for a
/// whole clause. A clause is everything from the start of the tokenization scope up to but not
/// including the next period, or through the end of the line, whichever comes first - a line may hold
/// several.
/// <para>
/// The exact behavior the Tokenizer had before <see cref="GlobalSettings.AllowPartialClauseMatches"/>
/// existed - so this is how a single type keeps that behavior once the setting turns it off globally. Only
/// meaningful while the setting is false; with it true every type already matches this way, making the
/// attribute redundant rather than wrong.
/// </para>
/// <para>
/// A type that wants to cover more than one clause is asking a different question, and wants a bare
/// <c>"."</c> nib instead (see <see cref="RegexGraph.SpansClauses"/>).
/// </para>
/// <para>
/// Also mutually exclusive with <see cref="DependentAttribute"/>: a dependent is never a top-level
/// tokenization candidate, so it is never subject to the requirement this opts out of.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AllowPartialClauseMatchAttribute : Attribute
{
}
