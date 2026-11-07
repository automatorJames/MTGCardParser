using MTGRulesIR.Lowering;
using MTGRulesIR.Model;

namespace ConsoleUtility;

/// <summary>
/// <c>overrides-todo rules.jsonl out.jsonl [count] [skip]</c>: the lines whose only opaque nodes come from card-specific
/// literals, as <see cref="LineOverride"/> skeletons - the card, the line's text, and its abilities as lowered, opaque
/// nodes left in place for the author to replace. Written in card-name order so batches can be taken in turn;
/// <c>count</c> and <c>skip</c> pick a batch. Also prints how many lines and cards are literal-blocked in all.
/// </summary>
static class OverridesTodo
{
    public static void Run(string rulesPath, string outPath, int count, int skip)
    {
        var literals = typeof(MTGGlyphs.GlyphDefinitions.ICardSpecific).Assembly.GetTypes()
            .Where(x => x.IsAssignableTo(typeof(MTGGlyphs.GlyphDefinitions.ICardSpecific)))
            .Select(x => x.Name)
            .ToHashSet(StringComparer.Ordinal);
        var overrides = LineOverrides.Default;

        List<(string Card, LineRules Line)> todo = [];
        var cards = new HashSet<string>();
        foreach (var raw in File.ReadLines(rulesPath))
        {
            var card = IrJson.Deserialize<CardRules>(raw);
            foreach (var line in card.Lines)
            {
                if (line.Opaque.Count == 0 || !line.Opaque.All(literals.Contains))
                    continue;
                if (overrides.TryGet(card.Name, line.Text, out _))
                    continue;
                todo.Add((card.Name, line));
                cards.Add(card.Name);
            }
        }

        todo.Sort((a, b) => string.CompareOrdinal(a.Card, b.Card));
        var batch = todo.Skip(skip).Take(count).ToList();
        using (var writer = new StreamWriter(outPath))
            foreach (var (name, line) in batch)
                writer.WriteLine(IrJson.Serialize(new LineOverride(name, line.Text, line.Abilities, $"literal: {string.Join(", ", line.Opaque.Distinct())}")));

        Console.WriteLine($"{todo.Count:N0} lines on {cards.Count:N0} cards are blocked only by card-specific literals; {overrides.Count:N0} overrides already written; wrote {batch.Count:N0} skeletons to {outPath}");
    }
}
