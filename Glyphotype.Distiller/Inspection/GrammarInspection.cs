using Glyphotype.RegexGeneration.Graph;
using Glyphotype.RegexGeneration.Graph.Nodes;

namespace Glyphotype.Distiller.Inspection;

/// <summary>One glyph nested in another's regex, how many times it's inlined there, and about how much of the regex it accounts for.</summary>
public sealed record ExpansionPart(string Name, int Uses, long Chars);

/// <summary>
/// How large one glyph's compiled regex is, and which nested glyphs make it so (see <see cref="GrammarInspection.GetExpansions"/>).
/// </summary>
/// <param name="RegexChars">The length of the glyph's pattern.</param>
/// <param name="NamedGroups">How many named groups it has - every property of every glyph inlined in it.</param>
/// <param name="Parts">The nested glyphs accounting for most of it, largest first.</param>
public sealed record GlyphExpansion(string Name, int RegexChars, int NamedGroups, IReadOnlyList<ExpansionPart> Parts);

/// <summary>Questions about a grammar itself, apart from any corpus.</summary>
public static class GrammarInspection
{
    /// <summary>
    /// Every glyph's regex size, with the nested glyphs that account for most of it: a glyph's regex inlines every
    /// glyph it refers to, which inline theirs, so a vocabulary of hundreds of members nested a few levels down
    /// appears in it once per path - and a few such paths make a pattern of megabytes. A part's share is
    /// approximate: its own regex size times the times it's inlined, which counts a glyph nested inside another
    /// part toward both.
    /// </summary>
    public static IReadOnlyList<GlyphExpansion> GetExpansions(GlyphGrammar grammar, int parts = 3)
    {
        var expansions = new List<GlyphExpansion>();
        var uses = new Dictionary<Type, Dictionary<Type, int>>();
        var groups = new Dictionary<Type, int>();

        foreach (var type in grammar.Types)
        {
            var graph = GlyphTypeCache.GetRegexGraph(type);

            var largest = UsesOf(type, uses)
                .Select(x => new ExpansionPart(Navigation.GetRegexSafeTypeName(x.Key), x.Value, x.Value * (long)GlyphTypeCache.GetRegexGraph(x.Key).BuiltRegex.MinifiedRegexLength))
                .OrderByDescending(x => x.Chars)
                .ThenBy(x => x.Name, StringComparer.Ordinal)
                .Take(parts)
                .ToList();

            expansions.Add(new(type.Name, graph.BuiltRegex.MinifiedRegexLength, GroupsOf(type, groups), largest));
        }

        return expansions;
    }

    /// <summary>
    /// How many times each glyph type is nested, at any depth, in <paramref name="type"/>'s graph - found from each nested
    /// type's own graph rather than by walking into it (a nested glyph's subtree is its type's graph over again), so a
    /// large grammar's inlined graphs needn't be built to report on them.
    /// </summary>
    static Dictionary<Type, int> UsesOf(Type type, Dictionary<Type, Dictionary<Type, int>> memo)
    {
        if (memo.TryGetValue(type, out var known))
            return known;

        var uses = new Dictionary<Type, int>();
        Count(GlyphTypeCache.GetRegexGraph(type).RootNode);
        memo[type] = uses;
        return uses;

        void Count(NamedGroupNode node)
        {
            foreach (var child in node.NamedGroupChildren)
            {
                if (child is GlyphNode glyph && glyph.Navigation.IsGlyphType)
                    uses[glyph.Navigation.NodeType] = uses.GetValueOrDefault(glyph.Navigation.NodeType) + 1;

                if (RegexGraph.IsNestedGlyph(child))
                    foreach (var (nested, count) in UsesOf(child.Navigation.NodeType, memo))
                        uses[nested] = uses.GetValueOrDefault(nested) + count;
                else
                    Count(child);
            }
        }
    }

    /// <summary>How many named groups <paramref name="type"/>'s graph has - every property of every glyph nested in it - found as <see cref="UsesOf"/> is.</summary>
    static int GroupsOf(Type type, Dictionary<Type, int> memo)
    {
        if (memo.TryGetValue(type, out var known))
            return known;

        var count = Count(GlyphTypeCache.GetRegexGraph(type).RootNode);
        memo[type] = count;
        return count;

        int Count(NamedGroupNode node) =>
            1 + node.NamedGroupChildren.Sum(child => RegexGraph.IsNestedGlyph(child) ? GroupsOf(child.Navigation.NodeType, memo) : Count(child));
    }
}
