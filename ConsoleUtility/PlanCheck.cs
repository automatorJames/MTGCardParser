using Glyphotype.RegexGeneration.Matching;
using Glyphotype.StaticRegistry;

namespace ConsoleUtility;

/// <summary><c>plancheck [count]</c>: builds every type's plan by walking its whole inlined graph and compares it, written out, with the plan built by calling nested glyphs' plans - printing the first types that differ, and where.</summary>
static class PlanCheck
{
    public static void Run(int count)
    {
        var grammar = GlyphGrammar.Default;
        var differing = 0;

        foreach (var type in grammar.Types)
        {
            var graph = GlyphTypeCache.GetRegexGraph(type);
            var compositional = graph.BuiltRegex.Plan.Render();
            var collector = new MatchPlan.Collector(graph.RootNode, compositional: false);
            graph.RootNode.AppendRegexBricks(collector);
            collector.Complete();
            var walked = collector.Plan.Render();

            if (walked == compositional)
                continue;

            differing++;

            if (differing > count)
                continue;

            var at = Enumerable.Range(0, Math.Min(walked.Length, compositional.Length)).FirstOrDefault(i => walked[i] != compositional[i], Math.Min(walked.Length, compositional.Length));
            Console.WriteLine($"{type.Name}: walked {walked.Length:N0} chars, compositional {compositional.Length:N0}; first difference at {at:N0}");
            Console.WriteLine($"  walked:        …{walked[Math.Max(0, at - 120)..Math.Min(walked.Length, at + 160)]}…");
            Console.WriteLine($"  compositional: …{compositional[Math.Max(0, at - 120)..Math.Min(compositional.Length, at + 160)]}…");
        }

        Console.WriteLine($"{differing} of {grammar.Types.Count} types differ");
    }
}
