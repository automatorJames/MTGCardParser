using MTGEngine.Core;

namespace MTGEngine.Agents;

/// <summary>
/// Plays legally and at random, with a little greed: lands get played, spells get cast when they can be, creatures
/// attack when they outmatch the defence, blocks happen when they trade well. Seeded, so a game replays. A baseline
/// for smoke tests and for measuring better agents against.
/// </summary>
public sealed class RandomAgent(int seed, double aggression = 0.7) : IDecisionMaker
{
    readonly Random _rng = new(seed);

    /// <summary>How often this turn each ability was chosen; past a few times an ability is left alone, so a free repeatable one can't fill a turn.</summary>
    readonly Dictionary<(int Turn, ObjectId Source, int Index), int> _activated = [];
    const int RepeatsPerTurn = 8;

    public DecisionResponse Decide(GameState state, Decision decision)
    {
        switch (decision)
        {
            case ChooseAction action:
            {
                var options = action.Options;
                var land = options.Select((x, i) => (x, i)).FirstOrDefault(p => p.x is PlayLand);
                if (land.x is not null)
                    return DecisionResponse.Choose(land.i);
                var spells = options.Select((x, i) => (x, i)).Where(p => p.x is CastSpell).ToList();
                if (spells.Count > 0 && _rng.NextDouble() < 0.9)
                    return DecisionResponse.Choose(spells[_rng.Next(spells.Count)].i);
                var abilities = options.Select((x, i) => (x, i))
                    .Where(p => p.x is TurnFaceUp || p.x is ActivateAbility a && _activated.GetValueOrDefault((state.Turn, a.Source, a.Index)) < RepeatsPerTurn).ToList();
                if (abilities.Count > 0 && _rng.NextDouble() < 0.3)
                {
                    var pick = abilities[_rng.Next(abilities.Count)];
                    if (pick.x is ActivateAbility chosen)
                        _activated[(state.Turn, chosen.Source, chosen.Index)] = _activated.GetValueOrDefault((state.Turn, chosen.Source, chosen.Index)) + 1;
                    return DecisionResponse.Choose(pick.i);
                }
                return DecisionResponse.Pass;
            }

            case ChooseTargets targets:
            {
                var count = targets.Max;
                var me = targets.Player;
                // Prefer opponents and their things for hostile prompts, own things for sacrifice/discard prompts.
                var hostile = targets.Prompt.StartsWith("Choose a target", StringComparison.OrdinalIgnoreCase);
                var ordered = targets.Candidates
                    .OrderBy(x => hostile ? (OwnedBy(state, x, me) ? 1 : 0) : (OwnedBy(state, x, me) ? 0 : 1))
                    .ThenBy(_ => _rng.Next())
                    .Take(Math.Max(targets.Min, Math.Min(count, _rng.Next(targets.Min, count + 1))))
                    .ToList();
                return DecisionResponse.Of(ordered);
            }

            case ChooseAttackers attackers:
            {
                var defender = attackers.Defenders.FirstOrDefault(x => x.IsPlayer).Player;
                var blockers = defender is null ? [] : state.Battlefield.Where(x => x.Controller == defender && !x.Tapped && Characteristics.Of(state, x).IsCreature).Select(x => Characteristics.Of(state, x)).ToList();
                var biggestBlocker = blockers.Count == 0 ? 0 : blockers.Max(x => x.Power);
                var chosen = attackers.Candidates.Where(id =>
                {
                    var traits = Characteristics.Of(state, state.Object(id));
                    var safe = blockers.Count == 0 || traits.Toughness > biggestBlocker || traits.HasKeyword("Flying") && blockers.All(b => !b.HasKeyword("Flying") && !b.HasKeyword("Reach"));
                    return safe || _rng.NextDouble() < aggression * 0.5;
                }).ToList();
                return DecisionResponse.Of(chosen);
            }

            case ChooseBlockers blockers:
            {
                var used = new HashSet<ObjectId>();
                var blockedAttackers = new HashSet<ObjectId>();
                List<(ObjectId, ObjectId)> blocks = [];
                foreach (var (blocker, attacker) in blockers.LegalBlocks.OrderBy(_ => _rng.Next()))
                {
                    if (used.Contains(blocker) || blockedAttackers.Contains(attacker))
                        continue;
                    var b = Characteristics.Of(state, state.Object(blocker));
                    var a = Characteristics.Of(state, state.Object(attacker));
                    var good = b.Toughness > a.Power || b.Power >= a.Toughness && b.Toughness > a.Power / 2;
                    var life = state.Player(blockers.Player).Life;
                    if (good || life <= 8 || _rng.NextDouble() < 0.15)
                    {
                        blocks.Add((blocker, attacker));
                        used.Add(blocker);
                        blockedAttackers.Add(attacker);
                    }
                }
                return new DecisionResponse(Blocks: blocks.ToImmutableList());
            }

            case YesNo:
                return new DecisionResponse(Yes: _rng.NextDouble() < 0.8);

            case ChooseOne one:
                return DecisionResponse.Choose(_rng.Next(one.Options.Count));

            case ChooseNumber number:
                return new DecisionResponse(Number: number.Max);

            default:
                return new DecisionResponse();
        }
    }

    static bool OwnedBy(GameState state, TargetRef target, PlayerId player) =>
        target.IsPlayer ? target.Player == player : state.Object(target.Object.Value)?.Controller == player;
}

/// <summary>Always passes and declines; the null agent for tests that drive one side.</summary>
public sealed class PassiveAgent : IDecisionMaker
{
    public DecisionResponse Decide(GameState state, Decision decision) =>
        decision switch
        {
            ChooseTargets targets => DecisionResponse.Of(targets.Candidates.Take(targets.Min)),
            _ => new DecisionResponse(),
        };
}

/// <summary>Plays a fixed script of choices, by decision type, then passes; for tests that want a particular line of play.</summary>
public sealed class ScriptedAgent : IDecisionMaker
{
    readonly Queue<Func<GameState, Decision, DecisionResponse>> _script = new();

    public ScriptedAgent Then(Func<GameState, Decision, DecisionResponse> step)
    {
        _script.Enqueue(step);
        return this;
    }

    public DecisionResponse Decide(GameState state, Decision decision)
    {
        while (_script.Count > 0)
        {
            var step = _script.Peek();
            var response = step(state, decision);
            if (response is not null)
            {
                _script.Dequeue();
                return response;
            }
            break;
        }

        return decision switch
        {
            ChooseTargets targets => DecisionResponse.Of(targets.Candidates.Take(targets.Max)),
            YesNo => new DecisionResponse(Yes: true),
            _ => new DecisionResponse(),
        };
    }
}
