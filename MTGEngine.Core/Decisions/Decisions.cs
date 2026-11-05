namespace MTGEngine.Core;

/// <summary>
/// A choice the game needs a player to make. Decisions are data: the engine builds one with its legal options, the
/// player's <see cref="IDecisionMaker"/> answers with a <see cref="DecisionResponse"/>, and nothing else about the
/// agent matters to the rules. Every decision lists what's legal, so an agent that picks any listed option is legal.
/// </summary>
public abstract record Decision(PlayerId Player, string Prompt);

/// <summary>What to do with priority: one of the listed actions (passing is always among them).</summary>
public sealed record ChooseAction(PlayerId Player, ImmutableList<LegalAction> Options) : Decision(Player, "Choose an action");

/// <summary>Pick between <paramref name="Min"/> and <paramref name="Max"/> of the candidates (targets, cards to discard, creatures to sacrifice, …).</summary>
public sealed record ChooseTargets(PlayerId Player, string Prompt, ImmutableList<TargetRef> Candidates, int Min, int Max) : Decision(Player, Prompt);

/// <summary>Which creatures attack (any subset of the candidates).</summary>
public sealed record ChooseAttackers(PlayerId Player, ImmutableList<ObjectId> Candidates, ImmutableList<TargetRef> Defenders) : Decision(Player, "Declare attackers");

/// <summary>Which creatures block which attackers: each legal pairing is listed; answer with a subset giving each blocker at most once.</summary>
public sealed record ChooseBlockers(PlayerId Player, ImmutableList<(ObjectId Blocker, ObjectId Attacker)> LegalBlocks) : Decision(Player, "Declare blockers");

/// <summary>A yes-or-no choice: "you may draw a card".</summary>
public sealed record YesNo(PlayerId Player, string Prompt) : Decision(Player, Prompt);

/// <summary>A choice among named options: modes, "choose a color".</summary>
public sealed record ChooseOne(PlayerId Player, string Prompt, ImmutableList<string> Options) : Decision(Player, Prompt);

/// <summary>A number: X, or how much to pay.</summary>
public sealed record ChooseNumber(PlayerId Player, string Prompt, int Min, int Max) : Decision(Player, Prompt);

public sealed record DecisionResponse(int Index = 0, ImmutableList<TargetRef> Targets = null, ImmutableList<ObjectId> Objects = null, ImmutableList<(ObjectId Blocker, ObjectId Attacker)> Blocks = null, bool Yes = false, int Number = 0)
{
    public static readonly DecisionResponse Pass = new(0);
    public static DecisionResponse Choose(int index) => new(index);
    public static DecisionResponse Of(IEnumerable<TargetRef> targets) => new(Targets: targets.ToImmutableList());
    public static DecisionResponse Of(IEnumerable<ObjectId> objects) => new(Objects: objects.ToImmutableList());
}

public interface IDecisionMaker
{
    DecisionResponse Decide(GameState state, Decision decision);
}

/// <summary>Something a player with priority may do.</summary>
public abstract record LegalAction;
public sealed record PassPriority : LegalAction { public override string ToString() => "pass"; }
public sealed record PlayLand(ObjectId Card) : LegalAction { public override string ToString() => $"play land {Card}"; }
public sealed record CastSpell(ObjectId Card, int MaxX = 0, bool Free = false, int Alternative = -1) : LegalAction { public override string ToString() => Free ? $"cast {Card} free" : Alternative >= 0 ? $"cast {Card} (alternative {Alternative})" : $"cast {Card}"; }
public sealed record ActivateAbility(ObjectId Source, int Index, string Text) : LegalAction { public override string ToString() => $"activate {Source}[{Index}]"; }
