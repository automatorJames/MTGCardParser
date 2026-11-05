using System.Collections.Immutable;
using MTGEngine.Agents;
using MTGEngine.Core;
using MTGRulesIR.Model;
using Zone = MTGEngine.Core.Zone;

namespace ConsoleUtility;

/// <summary>
/// <c>simulate rules.jsonl [games] [seed]</c>: plays games between random decks of the corpus's executable cards, with
/// random agents, and reports what went wrong - exceptions grouped by where they were thrown, with a deck seed to
/// replay - and what the engine skipped at run time though the interpret report counted it executable.
/// </summary>
static class Simulate
{
    public static void Run(string rulesPath, int games, int seed)
    {
        var cards = File.ReadLines(rulesPath)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(IrJson.Deserialize<CardRules>)
            .Where(card => card.Lines.All(line => line.EngineClosed && !line.Abilities.SelectMany(InterpreterSupport.Unsupported).Any()))
            .Select(CardDefinition.From)
            .Where(x => !x.IsLand && x.Types.Count > 0)
            .ToList();
        Console.WriteLine($"{cards.Count:N0} executable nonland cards");

        var lands = new[] { "Plains", "Island", "Swamp", "Mountain", "Forest" }
            .Select(name => new CardDefinition(name, ManaCost.Free, ["Basic"], ["Land"], [name], null, null, null, [], [], []))
            .ToList();
        var failures = new Dictionary<string, (int Count, int Seed, string Message, string Cards)>();
        var skips = new Dictionary<string, int>();
        var finished = 0;
        var turns = 0;

        for (var g = 0; g < games; g++)
        {
            var gameSeed = seed + g;
            var rng = new Random(gameSeed);
            Deck Build(string name)
            {
                var colors = lands.OrderBy(_ => rng.Next()).Take(2).ToList();
                List<CardDefinition> deck = [];
                for (var i = 0; i < 24; i++)
                    deck.Add(cards[rng.Next(cards.Count)]);
                for (var i = 0; i < 16; i++)
                    deck.Add(colors[i % 2]);
                return new Deck(name, deck.ToImmutableList());
            }
            var decks = new[] { Build("A"), Build("B") };
            var game = Game.Start(decks, [new RandomAgent(gameSeed), new RandomAgent(gameSeed + 7919)], gameSeed);
            try
            {
                game.Run(maxTurns: 25);
                finished++;
                turns += game.State.Turn;
            }
            catch (Exception ex)
            {
                var frame = ex.StackTrace?.Split('\n').Select(x => x.Trim()).FirstOrDefault(x => x.Contains("MTGEngine")) ?? "?";
                var key = $"{ex.GetType().Name} {frame}";
                var involved = string.Join(", ", game.State.Objects.Values.Where(x => x.Zone is Zone.Stack or Zone.Battlefield && !x.Card.IsLand).Select(x => x.Card.Name).Distinct().Take(8));
                failures[key] = failures.TryGetValue(key, out var seen) ? (seen.Count + 1, seen.Seed, seen.Message, seen.Cards) : (1, gameSeed, ex.Message, involved);
            }
            foreach (var (kind, count) in game.Skips)
                skips[kind] = skips.GetValueOrDefault(kind) + count;
        }

        Console.WriteLine($"{games:N0} games: {finished:N0} played through ({(finished == 0 ? 0 : turns / finished)} turns on average), {games - finished:N0} threw");
        foreach (var (key, (count, failedSeed, message, involved)) in failures.OrderByDescending(x => x.Value.Count))
            Console.WriteLine($"{count,6}x  {key}\n         seed {failedSeed}: {message}\n         in play: {involved}");
        Console.WriteLine("Skipped at run time (kind: why), most often first:");
        foreach (var (kind, count) in skips.OrderByDescending(x => x.Value).Take(40))
            Console.WriteLine($"{count,8}  {kind}");
    }
}
