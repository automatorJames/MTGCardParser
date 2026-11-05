namespace MTGEngine.Core;

/// <summary>
/// Permission to play a card from somewhere other than the hand: "exile the top card of your library; until end of
/// turn, you may play that card". Good while the card stays where it was put (its zone-change count is unchanged)
/// and until the turn it expires after; <see cref="Free"/> casts it without paying its mana cost.
/// </summary>
public sealed record PlayPermission(ObjectId Card, PlayerId Player, int ZoneChangeCount, int? ExpiresAfterTurn, bool Free);

/// <summary>A rule about combat a permanent has: "can't be blocked by walls", "can't attack unless defending player controls a swamp", "can't block alone".</summary>
public sealed record CombatRule(string Cant, Selector By, Condition If, IReadOnlyList<string> Modifiers, string Recipient, ObjectId Source, PlayerId Controller)
{
    /// <summary>The fewest blockers "can't be blocked except by N or more creatures" allows, and the most "can't be blocked by more than N" allows.</summary>
    public (int Min, int Max) BlockerCount =>
        By switch
        {
            ObjectSelector { Count: Fixed { Value: var least } } => (least, int.MaxValue),
            ObjectSelector { Count: Bound { Comparison: Comparison.Greater, Value: Fixed { Value: var most } } } => (0, most),
            _ => (0, int.MaxValue),
        };

    /// <summary>Whether <see cref="By"/> limits which creatures (rather than how many).</summary>
    public bool LimitsWhich => By is Self || By is ObjectSelector { Count: null };
}

public sealed partial class Game
{
    /// <summary>"You may play that card this turn": a permission for each card, or for "without paying its mana cost" with no duration, the chance to cast it now.</summary>
    public void GrantPlay(PlayCard play, IReadOnlyList<ObjectId> cards, PlayerId player, EffectContext ctx)
    {
        if (play.Free && play.Duration is null)
        {
            foreach (var id in cards)
            {
                var obj = State.Object(id);
                if (obj is null || obj.Card.IsLand || obj.Zone is Zone.Battlefield or Zone.Stack)
                    continue;
                if (!TargetsAvailable(SpellSlots(obj.Card), player, id) || !Decide(new YesNo(player, $"Cast {obj.Card.Name} without paying its mana cost?")).Yes)
                    continue;
                Cast(id, player, 0, free: true);
            }
            return;
        }

        var until = Interpreter.UntilTurn(play.Duration, State);
        var expires = until is null or -1 ? State.Turn : until;
        foreach (var id in cards)
            if (State.Object(id) is { } obj && obj.Zone is not (Zone.Battlefield or Zone.Stack))
                Emit(new PermissionGranted(new PlayPermission(id, player, obj.ZoneChangeCount, expires, play.Free)));
    }

    /// <summary>The permissions <paramref name="player"/> may use now: their card is still where it was when the permission was given.</summary>
    IEnumerable<(PlayPermission Permission, GameObject Card)> Permitted(PlayerId player) =>
        State.PermissionList
            .Where(x => x.Player == player)
            .Select(x => (Permission: x, Card: State.Object(x.Card)))
            .Where(x => x.Card is not null && x.Card.ZoneChangeCount == x.Permission.ZoneChangeCount && x.Card.Zone is not (Zone.Hand or Zone.Battlefield or Zone.Stack));

    /// <summary>Whether an activation or casting timing restriction allows acting now: "activate only if …", "only during your turn", "only during combat".</summary>
    bool TimingAllows(Timing timing, GameObject obj, PlayerId player)
    {
        switch (timing.Kind)
        {
            case nameof(TimingKind.SorcerySpeed) or nameof(TimingKind.InstantSpeed) or nameof(TimingKind.OnceEachTurn) or nameof(TimingKind.Once):
                return true;
            case "If":
                return Conditions.Evaluate(timing.If, new EffectContext(this, player, obj.Id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance))) == true;
            case "During" when timing.Phase is { } phase:
            {
                if (phase.Whose is { } whose)
                {
                    var ctx = new EffectContext(this, player, obj.Id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
                    var players = whose is PlayerSelector { Kind: PlayerKind.ThatPlayer } ? [player] : Resolver.Players(whose, ctx);
                    if (!players.Contains(State.ActivePlayer))
                        return false;
                }
                return DuringPhase(phase.Phase);
            }
            default:
                return false;
        }
    }

    /// <summary>Whether the game is in the named phase or step now ("Turn" is always).</summary>
    public bool DuringPhase(string phase) =>
        phase switch
        {
            null or "Turn" => true,
            "CombatPhase" or "Combat" => State.Step is Step.BeginCombat or Step.DeclareAttackers or Step.DeclareBlockers or Step.CombatDamage or Step.EndCombat,
            "DeclareBlockersStep" => State.Step == Step.DeclareBlockers,
            "DeclareAttackersStep" => State.Step == Step.DeclareAttackers,
            "MainPhase" => State.IsMainPhase,
            _ => Conditions.StepOf(phase) is { } step && State.Step == step,
        };

    public static readonly ImmutableHashSet<string> TimingKinds =
        [nameof(TimingKind.SorcerySpeed), nameof(TimingKind.InstantSpeed), nameof(TimingKind.OnceEachTurn), nameof(TimingKind.Once), "If", "During"];

    /// <summary>Whether a combat rule applies now: its condition holds (judged from its source).</summary>
    bool RuleHolds(CombatRule rule) =>
        rule.If is null || Conditions.Evaluate(rule.If, new EffectContext(this, rule.Controller, rule.Source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance))) == true;

    /// <summary>Whether <paramref name="other"/> is one of the creatures a rule's "by" names (for "can't be blocked by walls", "can't block creatures with power 2 or greater", "can't block it").</summary>
    bool RuleNames(CombatRule rule, GameObject other) =>
        rule.By switch
        {
            Self => other.Id == rule.Source,
            ObjectSelector selector => Filters.Matches(State, selector.Filter, other, rule.Controller, rule.Source, selector.Other ? rule.Source : null),
            _ => false,
        };

    /// <summary>Whether the uncounterable rule protects a spell: "this spell can't be countered" on it, or "creature spells can't be countered" on a permanent.</summary>
    bool CantBeCountered(GameObject spell)
    {
        static IReadOnlyList<string> OwnRule(Ability ability) =>
            CardDefinition.Unwrap(ability) switch
            {
                SpellAbility { Effect: CantBeTargeted { Objects: Self } rule } => rule.By,
                StaticAbility { Effect: CantBeTargeted { Objects: Self } rule } => rule.By,
                EffectLine { Effect: CantBeTargeted { Objects: Self } rule } => rule.By,
                _ => null,
            };
        if (spell.Card.Abilities.Any(a => OwnRule(a)?.Contains("Countered") == true))
            return true;
        foreach (var source in State.Battlefield)
            foreach (var ability in Characteristics.AbilitiesOf(State, source).OfType<StaticAbility>())
                if (ability.Effect is CantBeTargeted { Objects: ObjectSelector selector, By: var by } && by?.Contains("Countered") == true
                    && Filters.Matches(State, selector.Filter, spell, source.Controller, source.Id))
                    return true;
        return false;
    }
}
