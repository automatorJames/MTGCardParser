namespace Glyphotype.GlyphAnalysisDTOs.SpanAnalysis;

/// <summary>
/// One clause of a <see cref="ProcessedLine"/>: the units tokenized from it, and the <see cref="ClauseBreak"/> that
/// ends it - null for a line's last clause when the line doesn't end with a period.
/// <para>
/// A Glyph that spans clauses (see <see cref="RegexGraph.SpansClauses"/>) is never split across two of these: the
/// periods inside it are its own, not breaks between clauses, so the clauses it covers are one entry here.
/// </para>
/// </summary>
public sealed record LineClause(IReadOnlyList<CaptureUnit> Units, ClauseBreak Break)
{
    /// <summary>Every unit of the clause, its <see cref="Break"/> included as the last.</summary>
    public IEnumerable<CaptureUnit> UnitsWithBreak =>
        Break is null ? Units : Units.Append(Break);

    /// <summary>
    /// <paramref name="units"/> - one line's tokens, in order - grouped into clauses, each closed by the
    /// <see cref="ClauseBreak"/> after it.
    /// </summary>
    public static List<LineClause> Group(IEnumerable<CaptureUnit> units)
    {
        List<LineClause> clauses = [];
        List<CaptureUnit> current = [];

        foreach (var unit in units)
        {
            if (unit is ClauseBreak clauseBreak)
            {
                clauses.Add(new(current, clauseBreak));
                current = [];
            }
            else
                current.Add(unit);
        }

        if (current.Count > 0)
            clauses.Add(new(current, null));

        return clauses;
    }
}
