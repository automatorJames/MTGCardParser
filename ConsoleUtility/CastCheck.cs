using System.Collections.Immutable;
using MTGEngine.Agents;
using MTGEngine.Core;
using MTGRulesIR.Model;
using Zone = MTGEngine.Core.Zone;

namespace ConsoleUtility;

/// <summary>
/// <c>castcheck rules.jsonl [out.txt]</c>: casts each executable nonland card from the hand of a board with every
/// color of mana and creatures, artifacts, enchantments and graveyard cards on both sides, and lists the cards the
/// engine offers to cast that then don't reach the stack (a mismatch between what it offers and what it casts), the
/// cards whose casting threw, and the cards it never offers to cast (often fine: an unusual target or condition).
/// </summary>
static class CastCheck
{
    /// <summary>Takes as much as it may: every target it can (the opponent's first), yes to every question, the largest number; passes priority.</summary>
    sealed class EagerAgent : IDecisionMaker
    {
        public DecisionResponse Decide(GameState state, Decision decision) =>
            decision switch
            {
                ChooseTargets targets => DecisionResponse.Of(targets.Candidates
                    .OrderBy(x => x.IsPlayer ? (x.Player == targets.Player ? 3 : 1) : state.Object(x.Object.Value)?.Controller == targets.Player ? 2 : 0)
                    .Take(targets.Max)),
                YesNo => new DecisionResponse(Yes: true),
                ChooseNumber number => new DecisionResponse(Number: number.Max),
                ChooseAction => DecisionResponse.Pass,
                _ => new DecisionResponse(),
            };
    }

    static readonly string[] _basics = ["Plains", "Island", "Swamp", "Mountain", "Forest"];

    static CardDefinition Basic(string name) => new(name, ManaCost.Free, ["Basic"], ["Land"], [name], null, null, null, [], [], []);

    static CardDefinition Vanilla(string name, string cost, IEnumerable<string> types, int? power = null, int? toughness = null) =>
        new(name, ManaCost.Parse(cost), [], [.. types], [], power, toughness, null, [], [], ManaCost.Parse(cost).Colored.Keys.ToImmutableHashSet());

    public static void Run(string rulesPath, string outPath)
    {
        var cards = File.ReadLines(rulesPath)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(IrJson.Deserialize<CardRules>)
            .Where(card => card.Lines.All(line => line.EngineClosed && !line.Abilities.SelectMany(InterpreterSupport.Unsupported).Any()))
            .Select(CardDefinition.From)
            .Where(x => !x.IsLand && x.Types.Count > 0)
            .ToList();

        List<string> offeredNotCast = [], threw = [], notOffered = [], skipped = [], noEffect = [];
        var cast = 0;
        var clock = System.Diagnostics.Stopwatch.StartNew();
        List<string> slow = [];
        for (var n = 0; n < cards.Count; n++)
        {
            var card = cards[n];
            if (n % 1000 == 0)
                Console.Error.WriteLine($"{n:N0} cards, {clock.Elapsed.TotalSeconds:F0}s");
            var started = clock.Elapsed;
            var library = _basics.SelectMany(name => Enumerable.Repeat(Basic(name), 4)).ToImmutableList();
            var game = Game.Start([new Deck("A", library), new Deck("B", library)], [new EagerAgent(), new EagerAgent()], seed: 1, handSize: 0, shuffle: false);
            ObjectId Put(CardDefinition definition, int player, Zone zone)
            {
                var id = new ObjectId(game.State.NextObjectId);
                var owner = new PlayerId(player);
                game.Emit(new ObjectCreated(new GameObject(id, definition, owner, owner, Zone.Hand)));
                if (zone != Zone.Hand)
                    game.MoveToZone(id, zone, owner);
                return id;
            }
            try
            {
                foreach (var name in _basics)
                    for (var i = 0; i < 3; i++)
                        Put(Basic(name), 0, Zone.Battlefield);
                for (var player = 0; player < 2; player++)
                {
                    Put(Vanilla("Bears", "{1}{G}", ["Creature"], 2, 2), player, Zone.Battlefield);
                    Put(Vanilla("Construct", "{3}", ["Artifact", "Creature"], 3, 3), player, Zone.Battlefield);
                    Put(Vanilla("Totem", "{2}", ["Artifact"]), player, Zone.Battlefield);
                    Put(Vanilla("Charm", "{1}{W}", ["Enchantment"]), player, Zone.Battlefield);
                    Put(Basic("Forest"), player == 0 ? 1 : 0, Zone.Graveyard);
                    Put(Vanilla("Fallen", "{2}{B}", ["Creature"], 2, 1), player, Zone.Graveyard);
                    Put(Vanilla("Bolt", "{R}", ["Instant"]), player, Zone.Graveyard);
                }
                Put(Vanilla("Spare", "{4}", ["Creature"], 4, 4), 0, Zone.Hand);
                Put(Basic("Swamp"), 0, Zone.Hand);
                Put(Vanilla("Their Card", "{4}", ["Creature"], 4, 4), 1, Zone.Hand);
                var id = Put(card, 0, Zone.Hand);
                game.Emit(new SummoningSicknessCleared(new PlayerId(0)));
                game.Emit(new SummoningSicknessCleared(new PlayerId(1)));
                game.Emit(new TurnBegan(2, new PlayerId(0)));
                game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));

                var offer = game.LegalActions(new PlayerId(0)).OfType<CastSpell>().Where(x => x.Card == id).OrderBy(x => x.Alternative == -1 ? 0 : 1).FirstOrDefault();
                if (offer is null)
                {
                    notOffered.Add(card.Name);
                    continue;
                }
                var before = game.Log.Count;
                game.Take(offer, new PlayerId(0));
                if (game.Log.Skip(before).OfType<SpellCast>().Any(x => x.Item.Spell == id) || game.State.Object(id)?.Zone != Zone.Hand)
                    cast++;
                else
                {
                    offeredNotCast.Add($"{card.Name} ({offer})");
                    continue;
                }
                // What resolving it (and what it triggers) skipped at run time.
                var resolving = game.Log.Count;
                game.ResolveStack();
                var skips = game.Log.Skip(resolving).OfType<EffectSkipped>().Select(x => $"{x.Kind}: {x.Why}").Distinct().ToList();
                if (skips.Count > 0)
                    skipped.Add($"{card.Name}: {string.Join("; ", skips)}");
                // A spell that resolved without changing anything but where it is (often a wrong reference, or a condition that never holds).
                else if (!card.IsPermanentCard && !game.Log.Skip(resolving).Any(x => x is not (ResolutionBegan or Resolved or PriorityGiven or Passed or ModesChosen or MTGEngine.Core.ChoiceMade or ChoiceRecorded)
                    && !(x is ZoneMoved moved && moved.Object == id)))
                    noEffect.Add(card.Name);
            }
            catch (Exception ex)
            {
                threw.Add($"{card.Name}: {ex.GetType().Name} {ex.Message[..Math.Min(160, ex.Message.Length)]}");
            }
            finally
            {
                if ((clock.Elapsed - started).TotalSeconds > 1)
                {
                    slow.Add($"{card.Name} {(clock.Elapsed - started).TotalSeconds:F1}s");
                    Console.Error.WriteLine($"slow: {slow[^1]}");
                }
            }
        }

        var lines = new List<string>
        {
            $"{cards.Count:N0} executable nonland cards: {cast:N0} cast, {offeredNotCast.Count:N0} offered but not cast, {skipped.Count:N0} skipped something as they resolved, {noEffect.Count:N0} resolved with no effect, {threw.Count:N0} threw, {notOffered.Count:N0} not offered",
            "",
            "Offered but not cast:",
        };
        lines.AddRange(offeredNotCast.Select(x => "  " + x));
        lines.Add("");
        lines.Add("Threw:");
        lines.AddRange(threw.Select(x => "  " + x));
        lines.Add("");
        lines.Add("Skipped as it resolved:");
        lines.AddRange(skipped.Select(x => "  " + x));
        lines.Add("");
        lines.Add("Resolved with no effect:");
        lines.AddRange(noEffect.Select(x => "  " + x));
        lines.Add("");
        lines.Add("Slow (over a second):");
        lines.AddRange(slow.Select(x => "  " + x));
        lines.Add("");
        lines.Add("Not offered:");
        lines.AddRange(notOffered.Select(x => "  " + x));
        if (outPath is not null)
            File.WriteAllLines(outPath, lines);
        foreach (var line in lines.Take(3))
            Console.WriteLine(line);
        Console.WriteLine($"offered but not cast: {string.Join(", ", offeredNotCast.Take(30))}");
        Console.WriteLine($"threw: {string.Join(" | ", threw.Take(10))}");
    }
}
