using MTGEngine.Agents;

namespace MTGEngine.Tests;

/// <summary>The fixture cards (Cards/cards.jsonl, lowered rules copied from the corpus) as definitions, plus vanilla creatures and basic lands made by hand.</summary>
public static class Cards
{
    static readonly Lazy<Dictionary<string, CardDefinition>> _library = new(Load);

    static Dictionary<string, CardDefinition> Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Cards", "cards.jsonl");
        var cards = new Dictionary<string, CardDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in File.ReadLines(path))
        {
            var rules = IrJson.Deserialize<CardRules>(line);
            cards[rules.Name] = CardDefinition.From(rules);
        }
        return cards;
    }

    public static CardDefinition Named(string name) =>
        _library.Value.TryGetValue(name, out var card) ? card : throw new KeyNotFoundException($"No fixture card named {name}");

    public static IEnumerable<CardDefinition> All => _library.Value.Values;

    public static CardDefinition Vanilla(string name, string cost, int power, int toughness, params string[] subtypes) =>
        new(name, ManaCost.Parse(cost), [], ["Creature"], [.. subtypes], power, toughness, null, [], [], ManaCost.Parse(cost).Colored.Keys.ToImmutableHashSet());

    public static CardDefinition Basic(string name) =>
        new(name, ManaCost.Free, ["Basic"], ["Land"], [name], null, null, null, [], [], []);

    public static Deck DeckOf(string player, params (CardDefinition Card, int Count)[] cards) =>
        new(player, cards.SelectMany(x => Enumerable.Repeat(x.Card, x.Count)).ToImmutableList());
}

/// <summary>
/// Deterministic greed: plays a land, casts whatever it can (hostile targets at the opponent, helpful ones at
/// itself), attacks with everything, never blocks, always says yes. Tests set a hand up and read what happened.
/// </summary>
public sealed class GreedyAgent(bool attacks = true, bool blocks = false) : IDecisionMaker
{
    public DecisionResponse Decide(GameState state, Decision decision)
    {
        switch (decision)
        {
            case ChooseAction action:
            {
                for (var i = 0; i < action.Options.Count; i++)
                    if (action.Options[i] is PlayLand)
                        return DecisionResponse.Choose(i);
                for (var i = 0; i < action.Options.Count; i++)
                    if (action.Options[i] is CastSpell)
                        return DecisionResponse.Choose(i);
                for (var i = 0; i < action.Options.Count; i++)
                    if (action.Options[i] is ActivateAbility)
                        return DecisionResponse.Choose(i);
                return DecisionResponse.Pass;
            }
            case ChooseTargets targets:
            {
                var me = targets.Player;
                var hostile = targets.Prompt.StartsWith("Choose a target");
                var ordered = targets.Candidates
                    .OrderBy(x => Mine(state, x, me) == hostile ? 1 : 0)
                    .ThenBy(x => hostile == x.IsPlayer ? 1 : 0)
                    .Take(targets.Max)
                    .ToList();
                return DecisionResponse.Of(ordered);
            }
            case ChooseAttackers attackers:
                return DecisionResponse.Of(attacks ? attackers.Candidates : []);
            case ChooseBlockers blockers:
            {
                if (!blocks)
                    return new DecisionResponse(Blocks: []);
                var used = new HashSet<ObjectId>();
                var chosen = blockers.LegalBlocks.Where(x => used.Add(x.Blocker)).ToImmutableList();
                return new DecisionResponse(Blocks: chosen);
            }
            case YesNo:
                return new DecisionResponse(Yes: true);
            case ChooseNumber number:
                return new DecisionResponse(Number: number.Max);
            default:
                return new DecisionResponse();
        }
    }

    static bool Mine(GameState state, TargetRef target, PlayerId me) =>
        target.IsPlayer ? target.Player == me : state.Object(target.Object.Value)?.Controller == me;
}

public static class GameExtensions
{
    public static PlayerState P0(this Game game) => game.State.Player(new PlayerId(0));
    public static PlayerState P1(this Game game) => game.State.Player(new PlayerId(1));

    public static IEnumerable<GameObject> BattlefieldOf(this Game game, int player) =>
        game.State.Battlefield.Where(x => x.Controller == new PlayerId(player));

    public static GameObject Find(this Game game, string name, Zone? zone = null) =>
        game.State.Objects.Values.FirstOrDefault(x => x.Card.Name == name && (zone is null || x.Zone == zone));

    public static Game Started(Deck first, Deck second, IDecisionMaker firstAgent = null, IDecisionMaker secondAgent = null, int handSize = 7) =>
        Game.Start([first, second], [firstAgent ?? new GreedyAgent(), secondAgent ?? new PassiveAgent()], seed: 1, handSize: handSize, shuffle: false);
}
