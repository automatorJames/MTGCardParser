using Glyphotype.Distiller.Inspection;
using Glyphotype.StaticRegistry;

namespace ConsoleUtility;

/// <summary>
/// <c>expansion [count]</c>: the glyphs with the largest compiled regexes, and the nested glyphs that make them so -
/// what <c>list_glyphs sort=size</c> shows an agent, for the compiled grammar.
/// </summary>
static class Expansion
{
    public static void Run(int count)
    {
        var grammar = GlyphGrammar.Default;
        var expansions = GrammarInspection.GetExpansions(grammar, parts: 4);
        var topLevel = grammar.TopLevelTypes.Select(x => x.Name).ToHashSet();

        Console.WriteLine($"{grammar.Types.Count} types, {expansions.Sum(x => (long)x.RegexChars):N0} regex chars in all; the {count} largest:");
        Console.WriteLine($"  {"regex",11} {"groups",6}  glyph  <- largest parts (× times inlined, ~chars)");

        foreach (var expansion in expansions.OrderByDescending(x => x.RegexChars).Take(count))
            Console.WriteLine($"  {expansion.RegexChars,11:N0} {expansion.NamedGroups,6:N0}  {expansion.Name}{(topLevel.Contains(expansion.Name) ? "" : " (dependent)")}  <- {string.Join(", ", expansion.Parts.Select(p => $"{p.Name} ×{p.Uses} (~{p.Chars:N0})"))}");

        // The glyphs inlined most often across the whole grammar, weighted by their own size: the shared parts a change to would shrink the most.
        var shared = expansions
            .SelectMany(x => x.Parts.Select(p => (x.Name, Part: p)))
            .GroupBy(x => x.Part.Name)
            .Select(g => (Part: g.Key, Users: g.Select(x => x.Name).Distinct().Count(), Chars: g.Sum(x => x.Part.Chars)))
            .OrderByDescending(x => x.Chars)
            .Take(count)
            .ToList();

        Console.WriteLine();
        Console.WriteLine("Nested glyphs accounting for the most regex text across the grammar (as a largest part of some glyph):");

        foreach (var (part, users, chars) in shared)
            Console.WriteLine($"  {chars,13:N0} chars in {users,3} glyphs  {part}");
    }
}
