namespace Glyphotype.Tokenizers;

/// <summary>
/// What tokenizing one line tried: every place a candidate type was offered a match - by the line's own tokenizing,
/// and by every tokenizing a dynamic glyph did of part of it - and which types' regexes matched anywhere. All a
/// <see cref="TokenizationChange"/> needs to tell whether another grammar could tokenize the line differently.
/// </summary>
public sealed class TokenizationTrace
{
    [ThreadStatic]
    static TokenizationTrace _current;

    readonly List<Attempt> _attempts = [];
    readonly HashSet<Type> _matchedTypes = [];

    /// <summary>
    /// A place candidate types were offered a match: tokenizing <see cref="Text"/> had reached <see cref="Index"/>, within
    /// a scope ending at <see cref="EndIndex"/> and a clause ending at <see cref="ClauseEndIndex"/>, trying the types
    /// <see cref="ScopeToType"/> and <see cref="IncludeDependentTypes"/> allowed.
    /// </summary>
    internal readonly record struct Attempt(string Text, int Index, int EndIndex, int ClauseEndIndex, bool RequireWholeClauses, Type ScopeToType, bool IncludeDependentTypes);

    internal IReadOnlyList<Attempt> Attempts => _attempts;

    /// <summary>How many places candidate types were offered a match.</summary>
    public int AttemptCount => _attempts.Count;

    /// <summary>The types whose regex matched where they were tried - whether or not they went on to hydrate, or won the place.</summary>
    public IReadOnlySet<Type> MatchedTypes => _matchedTypes;

    /// <summary>Runs <paramref name="tokenize"/>, recording what it tries.</summary>
    public static TokenizationTrace Record(Action tokenize)
    {
        var outer = _current;
        var trace = _current = new TokenizationTrace();

        try
        {
            tokenize();
        }
        finally
        {
            _current = outer;
        }

        trace._attempts.TrimExcess();
        return trace;
    }

    /// <summary>Notes that the types <paramref name="scopeToType"/> and <paramref name="includeDependentTypes"/> allow are offered a match at <paramref name="index"/> - while a trace is recording.</summary>
    internal static void NoteAttempt(string text, int index, int endIndex, int clauseEndIndex, bool requireWholeClauses, Type scopeToType, bool includeDependentTypes) =>
        _current?._attempts.Add(new(text, index, endIndex, clauseEndIndex, requireWholeClauses, scopeToType, includeDependentTypes));

    /// <summary>Notes that <paramref name="type"/>'s regex matched where it was tried - while a trace is recording.</summary>
    internal static void NoteMatch(Type type) =>
        _current?._matchedTypes.Add(type);
}
