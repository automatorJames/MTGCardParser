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

    // A set, not a list: the same place is offered a match again whenever a dynamic slot's text recurs within a line
    // (or its resolution is replayed - see Replay), and one record of it says all there is to say.
    readonly HashSet<Attempt> _attempts = [];
    readonly HashSet<Type> _matchedTypes = [];

    /// <summary>
    /// A place candidate types were offered a match: tokenizing <see cref="Text"/> had reached <see cref="Index"/>, within
    /// a scope ending at <see cref="EndIndex"/> and a clause ending at <see cref="ClauseEndIndex"/>, trying the types
    /// <see cref="ScopeToType"/> and <see cref="IncludeDependentTypes"/> allowed.
    /// </summary>
    internal readonly record struct Attempt(string Text, int Index, int EndIndex, int ClauseEndIndex, bool RequireWholeClauses, Type ScopeToType, bool IncludeDependentTypes);

    internal IReadOnlyCollection<Attempt> Attempts => _attempts;

    /// <summary>How many distinct places candidate types were offered a match.</summary>
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

    /// <summary>
    /// Runs <paramref name="tokenize"/>, recording what it tries in a trace of its own - which is also added to the trace
    /// recording around it, as if it had been recorded there directly. For work whose outcome is memoized (see
    /// <see cref="Tokenizer.ResolveDynamic"/>): the trace is kept with the outcome, and <see cref="Replay"/>ed wherever
    /// the outcome is reused.
    /// </summary>
    internal static TokenizationTrace Capture(Action tokenize)
    {
        var trace = Record(tokenize);
        Replay(trace);
        return trace;
    }

    /// <summary>Adds everything <paramref name="trace"/> recorded to the trace recording now, if one is.</summary>
    internal static void Replay(TokenizationTrace trace)
    {
        if (_current is not { } current || trace is null)
            return;

        current._attempts.UnionWith(trace._attempts);
        current._matchedTypes.UnionWith(trace._matchedTypes);
    }

    /// <summary>Notes that the types <paramref name="scopeToType"/> and <paramref name="includeDependentTypes"/> allow are offered a match at <paramref name="index"/> - while a trace is recording.</summary>
    internal static void NoteAttempt(string text, int index, int endIndex, int clauseEndIndex, bool requireWholeClauses, Type scopeToType, bool includeDependentTypes) =>
        _current?._attempts.Add(new(text, index, endIndex, clauseEndIndex, requireWholeClauses, scopeToType, includeDependentTypes));

    /// <summary>Notes that <paramref name="type"/>'s regex matched where it was tried - while a trace is recording.</summary>
    internal static void NoteMatch(Type type) =>
        _current?._matchedTypes.Add(type);
}
