using Glyphotype.StaticRegistry;

namespace ConsoleUtility;

/// <summary><c>lengthcheck</c>: every type's pattern length as its plan's collector counted it, against the written-out pattern's - and the tokenizer's order of top-level types.</summary>
static class LengthCheck
{
    public static void Run()
    {
        var grammar = GlyphGrammar.Default;
        var differing = new List<(string Name, int Counted, int Written)>();

        foreach (var type in grammar.Types)
        {
            var built = GlyphTypeCache.GetRegexGraph(type).BuiltRegex;
            var written = built.MinifiedRegex.Length;

            if (written != built.MinifiedRegexLength)
                differing.Add((type.Name, built.MinifiedRegexLength, written));
        }

        foreach (var (name, counted, written) in differing.OrderBy(x => x.Written).Take(10))
            Console.WriteLine($"{name}: counted {counted:N0}, written {written:N0}");

        foreach (var (name, _, _) in differing.OrderBy(x => x.Written).Take(3))
        {
            var graph = GlyphTypeCache.GetRegexGraph(grammar.Types.First(x => x.Name == name));
            var collector = new Glyphotype.RegexGeneration.Matching.MatchPlan.Collector(graph.RootNode) { TraceText = new() };
            graph.RootNode.AppendRegexBricks(collector);
            var counted = collector.TraceText.ToString();
            var written = graph.BuiltRegex.MinifiedRegex;
            var at = Enumerable.Range(0, Math.Min(counted.Length, written.Length)).FirstOrDefault(i => counted[i] != written[i], Math.Min(counted.Length, written.Length));
            Console.WriteLine($"{name}: first difference at {at}");
            Console.WriteLine($"  written: …{written[Math.Max(0, at - 100)..Math.Min(written.Length, at + 120)]}…");
            Console.WriteLine($"  counted: …{counted[Math.Max(0, at - 100)..Math.Min(counted.Length, at + 120)]}…");
        }

        Console.WriteLine($"{differing.Count} of {grammar.Types.Count} types' lengths differ");
        var order = grammar.TopLevelTypes.Select(x => x.Name).ToList();
        Console.WriteLine($"ActionAndSubjectDoes at {order.IndexOf("ActionAndSubjectDoes")}, ReturnToBattlefield at {order.IndexOf("ReturnToBattlefield")} of {order.Count}");
    }
}
