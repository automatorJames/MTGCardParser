using Glyphotype.RegexGeneration.Matching;
using Glyphotype.StaticRegistry;

namespace ConsoleUtility;

/// <summary><c>plan TypeName [depth]</c>: a glyph type's match plan (see <see cref="MatchPlan"/>), leaves abbreviated, calls expanded to the given depth.</summary>
static class PlanDump
{
    public static void Run(string typeName, int depth)
    {
        var grammar = GlyphGrammar.Default;
        var type = grammar.Types.FirstOrDefault(x => x.Name == typeName) ?? throw new ArgumentException($"No type named {typeName}");
        var plan = GlyphTypeCache.GetRegexGraph(type).BuiltRegex.Plan;
        Console.WriteLine($"{typeName}: plan size {plan.Size}, key {plan.Key[..Math.Min(24, plan.Key.Length)]}");
        Print(plan.Root, 1, depth);
    }

    static void Print(PlanNode node, int indent, int depth)
    {
        var pad = new string(' ', indent * 2);

        switch (node)
        {
            case PlanNode.Leaf leaf:
                Console.WriteLine($"{pad}leaf{(Glyphotype.RegexGeneration.Matching.Matcher.IsDeterministic(leaf.Pattern) ? " (deterministic)" : "")} [{leaf.Pattern.Length}]: {Cut(leaf.Pattern, 160)}");
                break;

            case PlanNode.Sequence sequence:
                Console.WriteLine($"{pad}sequence of {sequence.Items.Length}");
                foreach (var item in sequence.Items)
                    Print(item, indent + 1, depth);
                break;

            case PlanNode.Alternation alternation:
                Console.WriteLine($"{pad}alternation of {alternation.Alternatives.Length}");
                foreach (var alternative in alternation.Alternatives)
                    Print(alternative, indent + 1, depth);
                break;

            case PlanNode.Call call:
                Console.WriteLine($"{pad}call {call.Name}{(call.Quantifier is Glyphotype.Quantifier.Optional ? "?" : call.Quantifier is Glyphotype.Quantifier.AnyNumber ? "*" : "")} -> plan size {call.Target.Size}, key {call.Target.Key[..Math.Min(24, call.Target.Key.Length)]}");
                if (depth > 0)
                    Print(call.Target.Root, indent + 1, depth - 1);
                break;
        }
    }

    static string Cut(string text, int length) => text.Length > length ? text[..length] + "…" : text;
}
