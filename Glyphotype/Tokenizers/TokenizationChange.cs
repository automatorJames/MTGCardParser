namespace Glyphotype.Tokenizers;

/// <summary>
/// How one grammar's tokenizing differs from another's, line by line - so a corpus tokenized by the first can be
/// brought up to date with the second by tokenizing again only the lines it could tokenize differently.
/// <para>
/// A line's tokenization depends on its own text and on which of its grammar's candidate types match where, tried in
/// what order - and a type rebuilt unchanged is the same type (see <see cref="Definitions.GrammarEmitter.Reuse"/>).
/// Each place a type is offered a match, the first whose regex matches and that hydrates takes it; one whose regex
/// doesn't match fails there before hydrating anything, whatever the grammar around it. So from what tokenizing a line
/// under the first grammar tried (its <see cref="TokenizationTrace"/>), the second tokenizes it the same unless:
/// <list type="bullet">
/// <item>a type only the first has matched somewhere in it;</item>
/// <item>types that matched in it are tried in another order; or</item>
/// <item>a type only the second has would match at one of the places a type was offered a match - until one does,
/// tokenizing under the second goes exactly as under the first, offering a match at the very same places.</item>
/// </list>
/// </para>
/// </summary>
public sealed class TokenizationChange
{
    readonly HashSet<Type> _removed;
    readonly (RegexGraph Graph, bool IsDependent)[] _added;
    readonly Dictionary<Type, int> _beforeOrder;
    readonly Dictionary<Type, int> _afterOrder;
    readonly bool _reordered;

    TokenizationChange(GlyphGrammar before, GlyphGrammar after)
    {
        var beforeCandidates = GetCandidateTypes(before);
        var afterCandidates = GetCandidateTypes(after);

        _beforeOrder = beforeCandidates.Select((type, index) => (type, index)).ToDictionary(x => x.type, x => x.index);
        _afterOrder = afterCandidates.Select((type, index) => (type, index)).ToDictionary(x => x.type, x => x.index);

        _removed = beforeCandidates.Where(x => !_afterOrder.ContainsKey(x)).ToHashSet();
        _added = afterCandidates
            .Where(x => !_beforeOrder.ContainsKey(x))
            .Select(x => (GlyphTypeCache.GetRegexGraph(x), after.DependentTypes.Contains(x)))
            .ToArray();

        _reordered = !beforeCandidates.Where(_afterOrder.ContainsKey).SequenceEqual(afterCandidates.Where(_beforeOrder.ContainsKey));
    }

    /// <summary>How <paramref name="after"/> tokenizes differently from <paramref name="before"/>.</summary>
    public static TokenizationChange Between(GlyphGrammar before, GlyphGrammar after) => new(before, after);

    /// <summary>Whether the grammars tokenize nothing differently: they try the same types, in the same order.</summary>
    public bool IsNone => _removed.Count == 0 && _added.Length == 0 && !_reordered;

    /// <summary>
    /// Whether <paramref name="line"/>, as the first grammar tokenized it, could tokenize differently under the second -
    /// if not, it tokenizes the same. A line with no <see cref="ProcessedLine.TokenizationTrace"/> always could.
    /// </summary>
    public bool MayDiffer(ProcessedLine line)
    {
        if (line.TokenizationTrace is not { } trace)
            return true;

        // A type the first grammar doesn't try either can only be from a trace of some other grammar's: nothing to go on.
        if (trace.MatchedTypes.Any(x => _removed.Contains(x) || !_beforeOrder.ContainsKey(x)))
            return true;

        if (_reordered && trace.MatchedTypes.Count > 1
            && !trace.MatchedTypes.OrderBy(x => _beforeOrder[x]).SequenceEqual(trace.MatchedTypes.OrderBy(x => _afterOrder[x])))
            return true;

        // Checking every added type at every place costs more than tokenizing the line again, past a few dozen - as when a
        // glyph many others inline is edited, and every one of them is rebuilt.
        if (_added.Length > _mostAddedToCheck)
            return true;

        return _added.Length > 0 && trace.Attempts.Any(attempt => _added.Any(added => CouldMatch(added.Graph, added.IsDependent, attempt)));
    }

    /// <summary>How many added types are worth checking a line against rather than tokenizing it again (see <see cref="MayDiffer"/>).</summary>
    const int _mostAddedToCheck = 64;

    /// <summary>Whether <paramref name="graph"/>'s type, offered a match at <paramref name="attempt"/>, would get past its regex - checked just as the <see cref="Tokenizer"/> checks it.</summary>
    static bool CouldMatch(RegexGraph graph, bool isDependent, TokenizationTrace.Attempt attempt)
    {
        if (isDependent && !attempt.IncludeDependentTypes)
            return false;

        if (attempt.ScopeToType is { } scope && !graph.RootGlyphType.IsAssignableTo(scope))
            return false;

        if (!graph.StartChars.CanStartWith(attempt.Text[attempt.Index]))
            return false;

        return Tokenizer.GetCandidateEnds(attempt.Text, attempt.EndIndex, attempt.ClauseEndIndex, attempt.RequireWholeClauses, graph.SpansClauses)
            .Any(end => graph.MatchesRegexAt(attempt.Text, attempt.Index, end, attempt.RequireWholeClauses));
    }

    /// <summary>Every type a grammar's tokenizer may try, in the order it tries them.</summary>
    static List<Type> GetCandidateTypes(GlyphGrammar grammar) =>
        grammar.TopLevelTypes.Concat(grammar.DependentTypes).ToList();
}
