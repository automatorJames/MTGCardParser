using System.Text.RegularExpressions;

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
/// <para>
/// A glyph whose own definition didn't change is still a new type when a glyph or vocabulary it refers to did (see
/// <see cref="Definitions.GrammarEmitter"/>): its regex inlines theirs. Such a type is <i>equivalent</i> to the one it
/// replaces where its regex differs only inside the groups of the types that changed - checked, not assumed - and on a
/// line where none of those groups' contents, old or new, matches anywhere, it tokenizes just as the one it replaces
/// did: a part of a pattern that matches nowhere in the text fails everywhere, whatever it is. So a line like that is
/// kept, though it was tokenized by types the second grammar no longer has (see <see cref="Supersedes"/>).
/// </para>
/// </summary>
public sealed class TokenizationChange
{
    readonly HashSet<Type> _removed;
    readonly (RegexGraph Graph, bool IsDependent)[] _added;
    readonly Dictionary<Type, int> _beforeOrder;
    readonly Dictionary<Type, int> _afterOrder;
    readonly bool _reordered;

    /// <summary>Each type of the first grammar equivalent to one of the second's, to that one.</summary>
    readonly Dictionary<Type, Type> _equivalents;

    /// <summary>What the changed types' groups, inlined in the equivalent types' regexes, match - old and new: a line any of these matches in could tokenize differently.</summary>
    readonly Regex[] _changedContents;

    TokenizationChange(GlyphGrammar before, GlyphGrammar after)
    {
        var beforeCandidates = GetCandidateTypes(before);
        var afterCandidates = GetCandidateTypes(after);

        _beforeOrder = beforeCandidates.Select((type, index) => (type, index)).ToDictionary(x => x.type, x => x.index);
        _afterOrder = afterCandidates.Select((type, index) => (type, index)).ToDictionary(x => x.type, x => x.index);

        (_equivalents, _changedContents) = FindEquivalents(before, after, beforeCandidates, afterCandidates);
        var replacements = _equivalents.Values.ToHashSet();

        _removed = beforeCandidates.Where(x => !_afterOrder.ContainsKey(x) && !_equivalents.ContainsKey(x)).ToHashSet();
        _added = afterCandidates
            .Where(x => !_beforeOrder.ContainsKey(x) && !replacements.Contains(x))
            .Select(x => (GlyphTypeCache.GetRegexGraph(x), after.DependentTypes.Contains(x)))
            .ToArray();

        _reordered = !beforeCandidates.Select(Replacement).Where(_afterOrder.ContainsKey).SequenceEqual(afterCandidates.Where(x => _beforeOrder.ContainsKey(x) || replacements.Contains(x)));

        _addedWithReplacements = new(() => [.. _added, .. afterCandidates.Where(replacements.Contains).Select(x => (GlyphTypeCache.GetRegexGraph(x), after.DependentTypes.Contains(x)))]);
    }

    /// <summary><see cref="_added"/> and the equivalent types' replacements: what could match anew on a line a changed type's group could match in.</summary>
    readonly Lazy<(RegexGraph Graph, bool IsDependent)[]> _addedWithReplacements;

    /// <summary>
    /// How many of <see cref="_addedWithReplacements"/> are worth checking a line against (see <see cref="CouldMatch"/>)
    /// rather than tokenizing it again: measured, a check takes a few microseconds per type per line, and tokenizing a
    /// line about as long as checking eighty.
    /// </summary>
    const int _mostReplacementsToCheck = 64;

    /// <summary>How <paramref name="after"/> tokenizes differently from <paramref name="before"/>.</summary>
    public static TokenizationChange Between(GlyphGrammar before, GlyphGrammar after) => new(before, after);

    /// <summary>Whether the grammars tokenize nothing differently: they try the same types, in the same order.</summary>
    public bool IsNone => _removed.Count == 0 && _added.Length == 0 && _equivalents.Count == 0 && !_reordered;

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
            && !trace.MatchedTypes.OrderBy(x => _beforeOrder[x]).SequenceEqual(trace.MatchedTypes.OrderBy(x => _afterOrder[Replacement(x)])))
            return true;

        // Where a changed type's group could match, an equivalent type is just a type removed and one added, as any other.
        // Every text tokenized is the line's or a part of it (a dynamic's slot), and a match is only ever found within one.
        var added = _added;

        if (_changedContents.Length > 0 && trace.Attempts.Select(x => x.Text).Distinct().Any(text => _changedContents.Any(x => x.IsMatch(text))))
        {
            // Checking every replacement at every place costs more than tokenizing the line again, past a few dozen.
            if (trace.MatchedTypes.Any(_equivalents.ContainsKey) || _addedWithReplacements.Value.Length > _mostReplacementsToCheck)
                return true;

            added = _addedWithReplacements.Value;
        }

        return added.Length > 0 && trace.Attempts.Any(attempt => added.Any(x => CouldMatch(x.Graph, x.IsDependent, attempt)));
    }

    /// <summary>
    /// Whether <paramref name="line"/> was tokenized by types other than <paramref name="grammar"/>'s - kept from an
    /// earlier grammar's tokenization because it tokenizes the same (see the remarks on equivalent types), but holding
    /// that grammar's types rather than these. A line with no <see cref="ProcessedLine.TokenizationTrace"/> isn't known to be.
    /// </summary>
    public static bool Supersedes(GlyphGrammar grammar, ProcessedLine line) =>
        line.TokenizationTrace is { } trace && trace.MatchedTypes.Any(x => !grammar.IsCandidate(x));

    Type Replacement(Type type) => _equivalents.GetValueOrDefault(type, type);

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

    /// <summary>
    /// The candidate types of <paramref name="before"/> rebuilt as new types of <paramref name="after"/> though their own
    /// definitions are the same, each to its replacement - where its regex differs only inside the groups of the glyphs
    /// and vocabularies whose definitions did change - and what those groups' contents match, old and new. None, if any
    /// of those contents could match differently within a part of a text than within the whole (see <see cref="Normalize"/>).
    /// </summary>
    static (Dictionary<Type, Type>, Regex[]) FindEquivalents(GlyphGrammar before, GlyphGrammar after, List<Type> beforeCandidates, List<Type> afterCandidates)
    {
        var replaced = beforeCandidates.Where(x => !after.IsCandidate(x)).ToDictionary(x => x.Name);
        var replacements = afterCandidates.Where(x => !before.IsCandidate(x) && replaced.ContainsKey(x.Name)).ToList();

        if (replacements.Count == 0)
            return ([], []);

        var changed = ChangedNames(before.ToDefinition(), after.ToDefinition());
        Dictionary<Type, Type> equivalents = [];
        HashSet<string> contents = [];

        foreach (var replacement in replacements)
        {
            var original = replaced[replacement.Name];

            // Offered a match in all the same places: as a top-level type or a dependent alike, and by every dynamic whose type filter it fits.
            if (changed.Contains(replacement.Name) || before.DependentTypes.Contains(original) != after.DependentTypes.Contains(replacement)
                || !Ancestry(original).SequenceEqual(Ancestry(replacement)))
                continue;

            var (originalGraph, replacementGraph) = (GlyphTypeCache.GetRegexGraph(original), GlyphTypeCache.GetRegexGraph(replacement));

            if (originalGraph.SpansClauses != replacementGraph.SpansClauses
                || !SameOutside(originalGraph.BuiltRegex, replacementGraph.BuiltRegex, changed, out var originalContents, out var replacementContents))
                continue;

            foreach (var content in originalContents.Concat(replacementContents))
            {
                if (Normalize(content) is not string normalized)
                    return ([], []);

                contents.Add(normalized);
            }

            equivalents[original] = replacement;
        }

        return (equivalents, [.. contents.Select(x => new Regex(x, RegexOptions.ExplicitCapture))]);
    }

    /// <summary>
    /// The names of what <paramref name="type"/> can be assigned to - what a dynamic's type filter is checked against -
    /// other than itself (see <see cref="NameOf"/>).
    /// </summary>
    static IEnumerable<string> Ancestry(Type type)
    {
        foreach (var name in type.GetInterfaces().Select(NameOf).Order(StringComparer.Ordinal))
            yield return name;

        for (var ancestor = type.BaseType; ancestor is not null; ancestor = ancestor.BaseType)
            yield return NameOf(ancestor);
    }

    /// <summary>
    /// <paramref name="type"/>'s name, with its type arguments' - by name alone, as a grammar names its types: one rebuilt
    /// is the same glyph in another assembly, and a compiled grammar's types are in namespaces an emitted one's aren't.
    /// </summary>
    static string NameOf(Type type) =>
        type.IsGenericType ? $"{type.Name}[{string.Join(",", type.GetGenericArguments().Select(NameOf))}]" : type.Name;

    /// <summary>The names of the glyphs and vocabularies defined differently - or only - in one definition or the other, documentation aside (which isn't emitted).</summary>
    static HashSet<string> ChangedNames(GrammarDefinition before, GrammarDefinition after)
    {
        var keys = before.Glyphs.Select(x => (x.Name, Key: DefinitionJson.Serialize(x with { Documentation = null })))
            .Concat(before.Vocabularies.Select(x => (x.Name, Key: DefinitionJson.Serialize(x))))
            .ToDictionary(x => x.Name, x => x.Key);

        var changed = after.Glyphs.Select(x => (x.Name, Key: DefinitionJson.Serialize(x with { Documentation = null })))
            .Concat(after.Vocabularies.Select(x => (x.Name, Key: DefinitionJson.Serialize(x))))
            .Where(x => !keys.Remove(x.Name, out var key) || key != x.Key)
            .Select(x => x.Name)
            .ToHashSet();

        // Whatever's left was only in the first.
        changed.UnionWith(keys.Keys);
        return changed;
    }

    /// <summary>
    /// Whether <paramref name="original"/> and <paramref name="replacement"/> are the same regex but for the contents of
    /// the outermost groups of the <paramref name="changed"/> types - and those contents, each regex's in its own order.
    /// </summary>
    static bool SameOutside(BuiltRegex original, BuiltRegex replacement, HashSet<string> changed, out List<string> originalContents, out List<string> replacementContents)
    {
        originalContents = replacementContents = null;

        if (original.TypedGroups is null || replacement.TypedGroups is null)
            return false;

        var originalRanges = OutermostChanged(original.TypedGroups, changed);
        var replacementRanges = OutermostChanged(replacement.TypedGroups, changed);

        if (originalRanges.Count != replacementRanges.Count)
            return false;

        var originalText = original.MinifiedRegex.AsSpan();
        var replacementText = replacement.MinifiedRegex.AsSpan();
        var (originalPosition, replacementPosition) = (0, 0);

        for (var i = 0; i <= originalRanges.Count; i++)
        {
            var (originalEnd, replacementEnd) = i < originalRanges.Count
                ? (originalRanges[i].ContentStart, replacementRanges[i].ContentStart)
                : (originalText.Length, replacementText.Length);

            if (i < originalRanges.Count && originalRanges[i].TypeName != replacementRanges[i].TypeName)
                return false;

            if (!originalText[originalPosition..originalEnd].SequenceEqual(replacementText[replacementPosition..replacementEnd]))
                return false;

            if (i < originalRanges.Count)
                (originalPosition, replacementPosition) = (originalRanges[i].ContentEnd, replacementRanges[i].ContentEnd);
        }

        originalContents = originalRanges.Select(x => original.MinifiedRegex[x.ContentStart..x.ContentEnd]).ToList();
        replacementContents = replacementRanges.Select(x => replacement.MinifiedRegex[x.ContentStart..x.ContentEnd]).ToList();
        return true;
    }

    /// <summary>The groups of the <paramref name="changed"/> types among <paramref name="groups"/> (in the order they open) that aren't inside another such group.</summary>
    static List<BuiltRegex.TypedGroup> OutermostChanged(IReadOnlyList<BuiltRegex.TypedGroup> groups, HashSet<string> changed)
    {
        List<BuiltRegex.TypedGroup> outermost = [];

        foreach (var group in groups)
            if (changed.Contains(group.TypeName) && (outermost.Count == 0 || group.ContentStart >= outermost[^1].ContentEnd))
                outermost.Add(group);

        return outermost;
    }

    /// <summary>
    /// <paramref name="regex"/> with its named groups made plain (which captures nothing, under
    /// <see cref="RegexOptions.ExplicitCapture"/>, and matches the same) - so the same content inlined in different
    /// places is one regex - or null if it holds anything whose matching depends on text outside what it matches, so
    /// could match within a part of a text where it doesn't within the whole: an anchor, a word boundary, a lookaround,
    /// a backreference or a conditional.
    /// </summary>
    internal static string Normalize(string regex)
    {
        var text = new System.Text.StringBuilder(regex.Length);
        var inClass = false;

        for (var i = 0; i < regex.Length; i++)
        {
            var c = regex[i];

            if (c == '\\' && i + 1 < regex.Length)
            {
                var escaped = regex[i + 1];

                if (!inClass && (escaped is 'b' or 'B' or 'A' or 'z' or 'Z' or 'G' or 'k' || escaped is >= '1' and <= '9'))
                    return null;

                text.Append(c).Append(escaped);
                i++;
                continue;
            }

            if (inClass)
            {
                // A class nested in a class is a subtraction: rare enough not to be worth following.
                if (c == '[')
                    return null;

                inClass = c != ']';
                text.Append(c);
                continue;
            }

            if (c == '[')
            {
                inClass = true;
                text.Append(c);

                // A ']' straight after the opening (or its negation) is a literal one.
                if (i + 1 < regex.Length && regex[i + 1] == '^')
                    text.Append(regex[++i]);

                if (i + 1 < regex.Length && regex[i + 1] == ']')
                    text.Append(regex[++i]);

                continue;
            }

            if (c is '^' or '$')
                return null;

            if (c == '(' && i + 2 < regex.Length && regex[i + 1] == '?')
            {
                var kind = regex[i + 2];

                if (kind is '=' or '!' or '(')
                    return null;

                if (kind is '<' or '\'')
                {
                    if (i + 3 < regex.Length && regex[i + 3] is '=' or '!')
                        return null;

                    var close = regex.IndexOf(kind == '<' ? '>' : '\'', i + 3);

                    if (close < 0)
                        return null;

                    text.Append("(?:");
                    i = close;
                    continue;
                }
            }

            text.Append(c);
        }

        return text.ToString();
    }

    /// <summary>Every type a grammar's tokenizer may try, in the order it tries them.</summary>
    static List<Type> GetCandidateTypes(GlyphGrammar grammar) =>
        grammar.TopLevelTypes.Concat(grammar.DependentTypes).ToList();
}
