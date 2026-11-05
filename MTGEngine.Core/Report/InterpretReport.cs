using System.Text;

namespace MTGEngine.Core;

/// <summary>
/// What of the lowered corpus the interpreter can execute: per line, whether every node is one the engine handles,
/// and across the corpus the node kinds that most often stand in the way - the engine's counterpart of the lowering
/// report, and the loop that drives engine work.
/// </summary>
public sealed class InterpretReport
{
    int _cards, _cardsExecutable, _lines, _linesExecutable, _linesClosed, _linesClosedExecutable;
    readonly Dictionary<string, (int Lines, string Example)> _blockers = new();

    public void Add(CardRules card)
    {
        _cards++;
        var allExecutable = true;

        foreach (var line in card.Lines)
        {
            _lines++;
            if (line.EngineClosed)
                _linesClosed++;

            var unsupported = line.Abilities.SelectMany(InterpreterSupport.Unsupported).Distinct().ToList();
            if (line.EngineClosed && unsupported.Count == 0)
            {
                _linesExecutable++;
                _linesClosedExecutable++;
            }
            else
            {
                allExecutable = false;
                foreach (var kind in line.EngineClosed ? unsupported : unsupported.Count == 0 ? ["(not engine-closed)"] : unsupported)
                {
                    var (count, example) = _blockers.GetValueOrDefault(kind);
                    _blockers[kind] = (count + 1, example ?? line.Text);
                }
            }
        }

        if (allExecutable)
            _cardsExecutable++;
    }

    public double LineFraction => _lines == 0 ? 0 : (double)_linesExecutable / _lines;

    public string ToText(int top = 60)
    {
        var text = new StringBuilder();
        text.AppendLine($"Cards: {_cards:N0}, executable {_cardsExecutable:N0} ({Percent(_cardsExecutable, _cards)})");
        text.AppendLine($"Lines: {_lines:N0}, engine-closed {_linesClosed:N0} ({Percent(_linesClosed, _lines)}), executable {_linesExecutable:N0} ({Percent(_linesExecutable, _lines)}); of the engine-closed, executable {Percent(_linesClosedExecutable, _linesClosed)}");
        text.AppendLine();
        text.AppendLine($"Node kinds the interpreter doesn't handle, by lines blocked (top {top} of {_blockers.Count}):");
        foreach (var (kind, (count, example)) in _blockers.OrderByDescending(x => x.Value.Lines).ThenBy(x => x.Key, StringComparer.Ordinal).Take(top))
            text.AppendLine($"{count,7} lines  {kind}  e.g. {Truncate(example, 90)}");
        return text.ToString();
    }

    static string Percent(int part, int whole) => whole == 0 ? "0%" : $"{100.0 * part / whole:F1}%";

    static string Truncate(string text, int length) => text is null ? "" : text.Length <= length ? text : text[..(length - 1)] + "…";
}

/// <summary>
/// The IR node kinds the interpreter, resolver, matcher and trigger code understand - kept in one place so the
/// report and the engine can't disagree. A node is supported when its type is, and for a few types when its
/// contents are (a pump's duration, a counter's kind, a mana option's choice).
/// </summary>
public static class InterpreterSupport
{
    static readonly HashSet<Type> _abilities =
    [
        typeof(KeywordLine), typeof(SpellAbility), typeof(TriggeredAbility), typeof(ActivatedAbility), typeof(StaticAbility),
        typeof(EffectLine), typeof(EnchantAbility), typeof(LabeledAbility), typeof(ReplacementAbility), typeof(LoyaltyAbility), typeof(ActivationLimits), typeof(Mode),
    ];

    static readonly HashSet<Type> _effects =
    [
        typeof(Sequence), typeof(Optional), typeof(Conditional), typeof(Lasting), typeof(Instead), typeof(WhenDone),
        typeof(DealDamage), typeof(Draw), typeof(GainLife), typeof(LoseLife), typeof(SetLifeTotal), typeof(Destroy), typeof(Exile),
        typeof(Sacrifice), typeof(Tap), typeof(Untap), typeof(PutCounters), typeof(RemoveCounters), typeof(CreateTokens), typeof(Pump),
        typeof(LoseAllAbilities), typeof(Animate), typeof(ChangeZone), typeof(ShuffleInto), typeof(Shuffle), typeof(Discard), typeof(Mill),
        typeof(CounterSpell), typeof(AddMana), typeof(GainControl), typeof(Fight), typeof(Attach), typeof(GameOutcome), typeof(ExtraTurn),
        typeof(LookAt), typeof(Reveal), typeof(LetVariable), typeof(ForEachOf), typeof(ChoiceOf), typeof(RemoveFromCombat),
        typeof(KeywordAction), typeof(UnlessPaid), typeof(CombatRestriction), typeof(GrantAbility), typeof(Delayed), typeof(SearchLibrary), typeof(DoesntUntap),
        typeof(ChooseCards), typeof(ChooseModes), typeof(Regenerate), typeof(EntersBattlefield), typeof(AsEventHappens),
    ];

    static readonly HashSet<Type> _selectors =
    [
        typeof(Self), typeof(ObjectSelector), typeof(PlayerSelector), typeof(Reference), typeof(Union),
    ];

    static readonly HashSet<Type> _quantities =
    [
        typeof(Fixed), typeof(Variable), typeof(CountOf), typeof(LifeTotal), typeof(Negated), typeof(Product), typeof(Arithmetic),
        typeof(Adjusted), typeof(Scaled), typeof(CharacteristicOf), typeof(Special), typeof(Bound), typeof(Extreme), typeof(DistinctCount), typeof(Devotion),
    ];

    static readonly HashSet<Type> _conditions =
    [
        typeof(And), typeof(Not), typeof(Controls), typeof(CountCondition), typeof(CompareCount), typeof(ThereAre), typeof(IsKind), typeof(ObjectIs),
        typeof(HasKeywordsCondition), typeof(HasCountersCondition), typeof(IsInZone), typeof(ItsPhase), typeof(Did), typeof(OutcomeCondition),
    ];

    static readonly HashSet<Type> _events =
    [
        typeof(ObjectEvent), typeof(PhaseEvent), typeof(SpellCastEvent), typeof(PlayerEvent), typeof(EitherEvent),
    ];

    static readonly HashSet<Type> _costs =
    [
        typeof(PayMana), typeof(TapSelf), typeof(UntapSelf), typeof(PayLife), typeof(ActionCost), typeof(CompoundCost),
    ];

    static readonly HashSet<Type> _predicates =
    [
        typeof(HasKeywords), typeof(HasStats), typeof(CharacteristicBound), typeof(Related), typeof(QualifierChoice), typeof(QualifiedKind),
        typeof(CastBy), typeof(WithCounters), typeof(Satisfying), typeof(ChosenBy),
    ];

    static readonly HashSet<Type> _durations =
    [
        typeof(ThisTurn), typeof(UntilPhase),
    ];

    static readonly HashSet<Type> _other =
    [
        typeof(Term), typeof(ObjectFilter), typeof(Qualifier), typeof(Controlled), typeof(ZoneRef), typeof(PhaseRef), typeof(DamagePortion),
        typeof(CounterSpec), typeof(TokenSpec), typeof(KeywordInstance), typeof(StatsMod), typeof(ManaAmount), typeof(ManaChoice), typeof(Timing), typeof(PayCost),
    ];

    static readonly HashSet<string> _objectEvents =
    [
        "EntersTheBattlefield", "EntersOrTransforms", "EntersUntapped", "Dies", "IsPutIntoAGraveyardFromTheBattlefield", "IsPutIntoAGraveyardFromAnywhere",
        "IsPutIntoAGraveyard", "LeavesTheBattlefield", "LeavesTheBattlefieldWithoutDying", "IsPutIntoExileFromTheBattlefield", "Attacks", "AttacksOrBlocks",
        "AttacksAlone", "AttacksYou", "AttacksAPlayer", "Blocks", "BlocksOrBecomesBlocked", "BlocksACreature", "BecomesBlocked", "BecomesTapped",
        "BecomesUntapped", "DealsCombatDamageToAPlayer", "DealsCombatDamageToAPlayerOrPlaneswalker", "DealsCombatDamageToAnOpponent", "DealsCombatDamageToYou",
        "DealsCombatDamage", "DealsDamage", "DealsDamageToAPlayer", "DealsDamageToAnOpponent", "DealsDamageToACreature", "DealsDamageToYou",
        "DealsDamageToAPlayerOrPlaneswalker", "IsDealtDamage", "IsDealtCombatDamage", "Cast", "Discarded", "CountersPut",
    ];

    static readonly HashSet<string> _qualifierVocabularies =
    [
        "CardType", "CreatureType", "LandType", "PermanentSubtype", "PlaneswalkerType", "ArtifactType", "EnchantmentType", "Supertype", "ManaColor",
        "ColorCount", "TapState", "CombatState", nameof(ObjectKind), "Distinction", "Keyword", "AttachedBy", "Kind", "Position", "Provenance",
    ];

    static readonly HashSet<string> _kindVocabularies =
    [
        nameof(ObjectKind), "CardType", "CreatureType", "LandType", "PermanentSubtype", "PlaneswalkerType", "ArtifactType", "EnchantmentType", "SpellSubtype", "Supertype", "PermanentKind",
    ];

    /// <summary>The names of the nodes under <paramref name="root"/> (itself included) that the interpreter can't handle.</summary>
    public static IEnumerable<string> Unsupported(IrNode root)
    {
        foreach (var node in MTGRulesIR.Lowering.NodeWalker.Nodes(root))
            if (Problem(node) is { } problem)
                yield return problem;
    }

    static string Problem(IrNode node)
    {
        var type = node.GetType();
        var name = type.Name;

        switch (node)
        {
            case Ability when !_abilities.Contains(type):
                return name;
            case Effect when !_effects.Contains(type):
                return name;
            case Selector when !_selectors.Contains(type):
                return name;
            case Quantity when !_quantities.Contains(type):
                return name;
            case Condition when !_conditions.Contains(type):
                return name;
            case Event when !_events.Contains(type):
                return name;
            case Cost when !_costs.Contains(type):
                return name;
            case Predicate when !_predicates.Contains(type):
                return name;
            case Duration when !_durations.Contains(type):
                return name;
            case Token when type != typeof(TokenSpec):
                return name;
            case ManaOption when type != typeof(ManaAmount) && type != typeof(ManaChoice):
                return name;
            case Ability or Effect or Selector or Quantity or Condition or Event or Cost or Predicate or Duration or Token or ManaOption:
                break;
            case not null when !_other.Contains(type):
                return name;
        }

        return node switch
        {
            Pump { Duration: { } duration } when !Keeps(duration) => $"Pump.Duration[{Describe(duration)}]",
            Animate { Duration: { } duration } when !Keeps(duration) => $"Animate.Duration[{Describe(duration)}]",
            Animate { Types: { Count: > 0 } } => "Animate.Types",
            Animate { Transforms: true } => "Animate.Transforms",
            ObjectSelector { Quantifier: Quantifier.Count, Count: null } => "ObjectSelector.Count",
            PlayerSelector { Kind: PlayerKind.Chosen or PlayerKind.EnchantedPlayer or PlayerKind.NoPlayer } player => $"PlayerSelector.{player.Kind}",
            Special { Kind: not ("ThatMany" or "ThatMuch" or "All" or "AnyNumber") } special => $"Special.{special.Kind}",
            Variable { Name: not ("X" or "x") } variable => $"Variable.{variable.Name}",
            ObjectEvent objectEvent when objectEvent.Events.All(x => !_objectEvents.Contains(x)) => $"ObjectEvent.{objectEvent.Events.FirstOrDefault()}",
            ObjectEvent { While: not null } => "ObjectEvent.While",
            ObjectEvent { By: not null } => "ObjectEvent.By",
            PhaseEvent phase when Conditions.StepOf(phase.At.Phase) is null => $"PhaseEvent.{phase.At.Phase}",
            PlayerEvent playerEvent when playerEvent.Event is not ("GainsLife" or "LosesLife" or "GainsOrLosesLife") => $"PlayerEvent.{playerEvent.Event}",
            Qualifier qualifier when !_qualifierVocabularies.Contains(qualifier.Vocabulary ?? "") => $"Qualifier[{qualifier.Vocabulary}]",
            Term term when !_kindVocabularies.Contains(term.Vocabulary ?? "") => $"Term[{term.Vocabulary}]",
            Related { Relation: not (Relation.OtherThan or Relation.AttachedTo or Relation.Among or Relation.Is) } related => $"Related.{related.Relation}",
            Controlled { Relation: not ("Control" or "Own") } controlled => $"Controlled.{controlled.Relation}",
            ZoneRef zone when !Enum.TryParse<Zone>(zone.Zone, out _) => $"ZoneRef.{zone.Zone}",
            ZoneRef { Position: not (null or "Top" or "Bottom") } zone => $"ZoneRef.Position.{zone.Position}",
            CounterSpec { Kind: null } => "CounterSpec.Kind",
            ManaChoice { Choice: not ("AnyColor" or "AnyOneColor") } choice => $"ManaChoice.{choice.Choice}",
            DealDamage { Divided: not null } => "DealDamage.Divided",
            Destroy { Manner: not (null or "CantBeRegenerated") } destroy => $"Destroy.{destroy.Manner}",
            Exile { Until: not null } => "Exile.Until",
            GameOutcome { Result: not ("Win" or "Lose") } outcome => $"GameOutcome.{outcome.Result}",
            TriggeredAbility { If: { } condition } when Problem(condition) is { } problem => problem,
            KeywordAction action when action.Actions.Any(x => !KeywordActions.Supported.Contains(x.Keyword)) => $"KeywordAction.{action.Actions.First(x => !KeywordActions.Supported.Contains(x.Keyword)).Keyword}",
            KeywordAction { Other: { Count: > 0 } } => "KeywordAction.Other",
            UnlessPaid { Payment.Cost: not (PayMana or PayLife) } unless => $"UnlessPaid[{unless.Payment.Cost?.GetType().Name}]",
            UnlessPaid { Payment.PerEach: not null } => "UnlessPaid.PerEach",
            CombatRestriction { If: not null } => "CombatRestriction.If",
            CombatRestriction { By: not null } => "CombatRestriction.By",
            CombatRestriction { Modifiers: { Count: > 0 } } => "CombatRestriction.Modifiers",
            CombatRestriction { Duration: { } duration } when !Keeps(duration) => $"CombatRestriction.Duration[{Describe(duration)}]",
            SearchLibrary { Zones: not (null or "Library") } search => $"SearchLibrary.{search.Zones}",
            EntersBattlefield enters when enters.States?.Any(x => x != "Tapped") == true => $"EntersBattlefield.{enters.States.First(x => x != "Tapped")}",
            EntersBattlefield { Duration: not null } => "EntersBattlefield.Duration",
            ReplacementAbility { Effect: not (EntersBattlefield or AsEventHappens) } replacement => $"ReplacementAbility[{replacement.Effect.GetType().Name}]",
            ReplacementAbility { Effect: AsEventHappens { When: not ObjectEvent } } => "AsEventHappens.When",
            AsEventHappens { When: ObjectEvent happens } when happens.Events.All(x => x is not ("EntersTheBattlefield" or "EntersOrTransforms")) => $"AsEventHappens.{happens.Events.FirstOrDefault()}",
            Timing timing when timing.Kind is not (nameof(TimingKind.SorcerySpeed) or nameof(TimingKind.InstantSpeed) or nameof(TimingKind.OnceEachTurn) or nameof(TimingKind.Once)) => $"Timing.{timing.Kind}",
            ActivationLimits { ForCasting: true } => "ActivationLimits.ForCasting",
            PayCost { Plus: not null } => "PayCost.Plus",
            Mode { Cost: not null } => "Mode.Cost",
            ActivatedAbility activated when activated.Costs.Any(x => x is ActionCost { Action: not (Sacrifice or Tap or Discard or PayCost or Exile or RemoveCounters) }) => "ActivatedAbility.Costs",
            KeywordLine line when line.Other is { Count: > 0 } => "KeywordLine.Other",
            StaticAbility statics when !IsStaticHandled(statics.Effect) => $"StaticAbility[{statics.Effect.GetType().Name}]",
            _ => null,
        };
    }

    static bool IsStaticHandled(Effect effect) =>
        effect switch
        {
            Pump { Duration: null } => true,
            CombatRestriction { Duration: null, If: null, By: null } => true,
            GrantAbility => true,
            LoseAllAbilities => true,
            DoesntUntap { If: null } => true,
            Sequence sequence => sequence.Steps.All(IsStaticHandled),
            _ => false,
        };

    static bool Keeps(Duration duration) =>
        duration switch
        {
            ThisTurn => true,
            UntilPhase { Phase.Phase: "EndOfTurn" or "EndStep" or "Cleanup" or "End" } => true,
            UntilPhase { Phase.Phase: "Turn", Phase.Next: true } => true,
            UntilPhase { Phase.Phase: "Untap" or "Upkeep" or "YourNextTurn" } => true,
            _ => false,
        };

    static string Describe(Duration duration) =>
        duration is UntilPhase until ? $"UntilPhase.{until.Phase.Phase}" : duration.GetType().Name;
}
