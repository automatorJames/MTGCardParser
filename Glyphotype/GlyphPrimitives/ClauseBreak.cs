namespace Glyphotype.GlyphPrimitives;

/// <summary>
/// The period that separates one clause from the next, as a first-class token rather than leftover text -
/// along with any stray closing parentheses or quotes straight after it. A period inside parenthesized or quoted
/// text ends a clause nested in the one around it, not that one (see <see cref="Enclosures"/> and <see cref="Depth"/>).
/// <para>
/// Synthesized directly by the <see cref="Tokenizers.Tokenizer"/> (like <see cref="UnmatchedString"/>,
/// and for the same reason) instead of competing as a top-level type: a clause break sits *between*
/// clauses, never at a clause's start, so under the whole-clause rule it could never be a candidate
/// anyway - and being registered would let some other type's regex shadow it. Making it a
/// <see cref="CaptureUnit"/> rather than a <see cref="Glyph"/> is also what keeps the registry's own type
/// discovery from picking it up as a top-level candidate.
/// </para>
/// <para>
/// Exists so that a period stops being reported as unmatched text. It isn't unmodeled - it's punctuation
/// the grammar treats as structure - so counting it as unmatched only pollutes the signal that says which
/// text still needs a Glyph written for it.
/// </para>
/// </summary>
public class ClauseBreak : CaptureUnit
{
    public ClauseBreak()
    {
    }

    static readonly ClauseBreakNode _rootNode = CreateSharedRootNode(new ClauseBreakNode(null, new(typeof(ClauseBreak))));

    public ClauseBreak(string sourceText, int index, int length, int depth = 0)
    {
        InitializeSpanContext(_rootNode, sourceText, index, length);
        Depth = depth;
    }

    /// <summary>
    /// How many enclosures (see <see cref="Enclosures"/>) this break sits inside: 0 for one ending a clause of the
    /// line itself, more for one ending a clause nested inside parenthesized or quoted text - "(this creature can't
    /// attack.)" - which ends that nested clause but not the clause around it (see <see cref="LineClause.Group"/>).
    /// </summary>
    public int Depth { get; }

    /// <summary>The one character that ends a clause. Every period outside an enclosure is a clause break - the Tokenizer splits on each (see <see cref="Enclosures"/>).</summary>
    public const char Period = '.';

    /// <summary>
    /// What may follow a clause-ending <see cref="Period"/> and still belong to the same break: a closing
    /// parenthesis or quote that closes nothing (see <see cref="Enclosures"/> - one that closes an enclosure would put
    /// the period inside it). Left behind, it would open the next clause as a lone ")" or "\"" of unmatched text.
    /// </summary>
    static readonly char[] _closingChars = [')', '"'];

    /// <summary>The length of the break starting at the <see cref="Period"/> at <paramref name="index"/>: the period plus any closing characters straight after it, up to <paramref name="endIndex"/>.</summary>
    public static int LengthAt(string sourceText, int index, int endIndex)
    {
        int length = 1;

        while (index + length < endIndex && _closingChars.Contains(sourceText[index + length]))
            length++;

        return length;
    }

    /// <summary>
    /// <paramref name="nib"/> as the nibs it stands for: a plain literal nib with a period inside it (e.g.
    /// <c>"by it. they can't be regenerated"</c>) is split around each period into the text before it, a bare
    /// <c>"."</c> nib (the clause-break nib - see <see cref="RegexGraph.SpansClauses"/>), and the text after it.
    /// Any other nib is returned as is. Literal text either side keeps the spaces it was written with, so the
    /// split matches exactly what the unsplit text would have under any <see cref="Joiner"/>; a piece that's only
    /// whitespace is dropped, the joiner supplying the space it stood for.
    /// <para>
    /// Whether a grammar allows such a nib at all is <see cref="GlobalSettings.AllowPeriodsInLiteralNibs"/>; where
    /// it does, this is all that's needed for one to work.
    /// </para>
    /// </summary>
    public static IEnumerable<Nib> SplitAtPeriods(Nib nib)
    {
        if (!IsSplittable(nib))
        {
            yield return nib;
            yield break;
        }

        var pieces = nib.Text.Split(Period);

        for (int i = 0; i < pieces.Length; i++)
        {
            if (i > 0)
                yield return new Nib(Period.ToString());

            // The text before a period binds tight to it, so its trailing spaces are dropped; the text after
            // keeps its leading ones, which is what tells the joiner not to add another.
            var piece = i < pieces.Length - 1 ? pieces[i].TrimEnd() : pieces[i];

            if (!string.IsNullOrWhiteSpace(piece))
                yield return new Nib(piece);
        }
    }

    /// <summary>
    /// <see cref="SplitAtPeriods(Nib)"/> around only the periods outside an enclosure the glyph writes itself -
    /// <paramref name="inside"/> flags each of the nib's characters (see <see cref="Enclosures.Inside(IReadOnlyList{Nib})"/>).
    /// A period inside one ends a clause nested in the glyph's own text, not a clause of the line, so it stays in its
    /// piece, and the piece is marked enclosed: its periods match only enclosed ones (see <see cref="Enclosures"/>).
    /// </summary>
    public static IEnumerable<(Nib Nib, bool Enclosed)> SplitAtPeriods(Nib nib, bool[] inside)
    {
        if (nib.GetType() != typeof(Nib))
        {
            yield return (nib, inside.Length > 0 && inside[0]);
            yield break;
        }

        var text = nib.Text;
        var breaks = Enumerable.Range(0, text.Length).Where(i => text[i] == Period && !inside[i]).ToList();

        // A bare "." nib is the clause-break nib itself, and a nib with no outside period has nothing to split.
        if (breaks.Count == 0 || text == Period.ToString())
        {
            yield return (nib, text.Contains(Period) && breaks.Count == 0);
            yield break;
        }

        int start = 0;

        foreach (var index in breaks.Append(text.Length))
        {
            // As in SplitAtPeriods(Nib): the text before a period drops its trailing spaces; the text after keeps its leading ones.
            var piece = index < text.Length ? text[start..index].TrimEnd() : text[start..];

            if (!string.IsNullOrWhiteSpace(piece))
                yield return (new Nib(piece), piece.Contains(Period));

            if (index < text.Length)
                yield return (new Nib(Period.ToString()), false);

            start = index + 1;
        }
    }

    /// <summary>Whether <paramref name="glyphType"/>'s last nib is literal text ending with a period (a bare <c>"."</c> nib included).</summary>
    public static bool EndsWithPeriod(Type glyphType) =>
        GlyphTypeCache.GetConfiguration(glyphType).Nibs.LastOrDefault() is { } last
        && last.GetType() == typeof(Nib)
        && last.Text.TrimEnd().EndsWith(Period);

    /// <summary>
    /// Whether <paramref name="glyphType"/>'s closing period (see <see cref="EndsWithPeriod"/>) is redundant, so it's
    /// dropped rather than refused: true unless the type is <see cref="DependentAttribute"/> or
    /// <see cref="AllowPartialClauseMatchAttribute"/>. Any other top-level match already has to end where a clause
    /// does, and the period there is emitted as a <see cref="ClauseBreak"/> of its own - so writing it out says
    /// nothing the whole-clause rule doesn't. A dependent's closing period instead falls inside its parent's match,
    /// and a partial match's says where it may stop; in both it's a real constraint.
    /// <para>
    /// A type that's also another glyph's property is in the same position as a dependent. That depends on the rest
    /// of the grammar, so <see cref="GlyphGrammar"/> refuses it (see <see cref="Glyph.GetPeriodError"/>).
    /// </para>
    /// </summary>
    public static bool IsTrailingPeriodRedundant(Type glyphType) =>
        !glyphType.IsDefined(typeof(DependentAttribute)) && !glyphType.IsDefined(typeof(AllowPartialClauseMatchAttribute));

    /// <summary><paramref name="nib"/> without the period(s) it ends with, or null when nothing's left (a bare <c>"."</c>).</summary>
    public static Nib WithoutTrailingPeriod(Nib nib)
    {
        var text = nib.Text.TrimEnd().TrimEnd(Period).TrimEnd();

        return text.Length == 0 ? null : new Nib(text);
    }

    /// <summary>Whether <paramref name="nib"/> is a plain literal nib that <see cref="SplitAtPeriods"/> splits - one with a period in it that isn't already the bare clause-break nib.</summary>
    public static bool IsSplittable(Nib nib) =>
        nib.GetType() == typeof(Nib) && nib.Text.Contains(Period) && nib.Text != Period.ToString();
}
