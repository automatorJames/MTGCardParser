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
    readonly Dictionary<string, (int Cards, string Example)> _soleBlockers = new();

    public void Add(CardRules card)
    {
        _cards++;
        var allExecutable = true;
        var cardBlockers = new HashSet<string>();

        foreach (var line in card.Lines)
        {
            _lines++;
            if (line.EngineClosed)
                _linesClosed++;

            // The engine drops a handled or casting keyword's reminder text, so its lowering doesn't need running.
            var unsupported = CardDefinition.WithoutCastingReminders(line).SelectMany(InterpreterSupport.Unsupported).Concat(InterpreterSupport.OnPermanent(card, line)).Concat(InterpreterSupport.PoolRestrictions(line)).Distinct().ToList();
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
                    cardBlockers.Add(kind);
                    var (count, example) = _blockers.GetValueOrDefault(kind);
                    _blockers[kind] = (count + 1, example ?? line.Text);
                }
            }
        }

        if (allExecutable)
            _cardsExecutable++;
        else if (cardBlockers.Count == 1)
        {
            var kind = cardBlockers.Single();
            var (count, example) = _soleBlockers.GetValueOrDefault(kind);
            _soleBlockers[kind] = (count + 1, example ?? card.Name);
        }
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
        text.AppendLine();
        text.AppendLine($"Node kinds that are a card's only blocker, by cards they'd make executable (top {top} of {_soleBlockers.Count}):");
        foreach (var (kind, (count, example)) in _soleBlockers.OrderByDescending(x => x.Value.Cards).ThenBy(x => x.Key, StringComparer.Ordinal).Take(top))
            text.AppendLine($"{count,7} cards  {kind}  e.g. {example}");
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
        typeof(EffectLine), typeof(EnchantAbility), typeof(LabeledAbility), typeof(FlavorAbility), typeof(ReplacementAbility), typeof(LoyaltyAbility), typeof(ActivationLimits), typeof(Mode),
        typeof(ChapterAbility), typeof(AdditionalCost), typeof(AlternativeCost), typeof(DieResultRow), typeof(LevelRange), typeof(StationThresholdRow), typeof(StatsLine),
    ];

    static readonly HashSet<Type> _effects =
    [
        typeof(Sequence), typeof(Optional), typeof(Conditional), typeof(Lasting), typeof(Instead), typeof(WhenDone),
        typeof(DealDamage), typeof(Draw), typeof(GainLife), typeof(LoseLife), typeof(SetLifeTotal), typeof(Destroy), typeof(Exile),
        typeof(Sacrifice), typeof(Tap), typeof(Untap), typeof(TapOrUntap), typeof(PutCounters), typeof(RemoveCounters), typeof(CreateTokens), typeof(Pump),
        typeof(LoseAllAbilities), typeof(Animate), typeof(ChangeZone), typeof(ShuffleInto), typeof(Shuffle), typeof(Discard), typeof(Mill),
        typeof(CounterSpell), typeof(AddMana), typeof(GainControl), typeof(Fight), typeof(Attach), typeof(GameOutcome), typeof(ExtraTurn),
        typeof(LookAt), typeof(Reveal), typeof(LetVariable), typeof(ForEachOf), typeof(ChoiceOf), typeof(RemoveFromCombat),
        typeof(KeywordAction), typeof(UnlessPaid), typeof(CombatRestriction), typeof(GrantAbility), typeof(Delayed), typeof(SearchLibrary), typeof(DoesntUntap),
        typeof(ChooseCards), typeof(ChooseModes), typeof(Regenerate), typeof(EntersBattlefield), typeof(AsEventHappens), typeof(PayCost),
        typeof(ChooseEffect), typeof(BecomesAspect), typeof(PreventDamage), typeof(RedirectDamage), typeof(CostModification),
        typeof(PlayCard), typeof(CantBeRegenerated), typeof(CantBeTargeted), typeof(MustCombat), typeof(SetStats), typeof(Replacement),
        typeof(Otherwise), typeof(StillOfKind), typeof(Remains), typeof(RevealUntil), typeof(CantPlay), typeof(GetEnergy), typeof(CanAsThough), typeof(Convoke),
        typeof(CopySpell), typeof(DistributeCounters), typeof(MoveCounters), typeof(AssignsNoCombatDamage), typeof(ExchangeControl), typeof(Phasing), typeof(MustBlock), typeof(MustBeBlocked), typeof(DoubleCounters), typeof(DoubleStats), typeof(SwitchStats), typeof(ChangeTargets), typeof(CantBeActivated), typeof(CanBlock), typeof(CreateEmblem),
        typeof(FlipCoin), typeof(RollDice), typeof(CastAsThoughFlash), typeof(MustBlock), typeof(MayNotUntap), typeof(Skip), typeof(TurnFace),
        typeof(GainClassLevel), typeof(BecomeCopy), typeof(MayPlayFrom), typeof(ManaRestriction), typeof(ExtraLandPlays),
        typeof(AssignsDamageByToughness), typeof(MaximumHandSize), typeof(NoMaximumHandSize), typeof(GainAbilitiesOf), typeof(ProtectionDoesntRemoveThis), typeof(AssignsDamageAsThoughUnblocked), typeof(PlayWithHandRevealed),
    ];

    static readonly HashSet<Type> _selectors =
    [
        typeof(Self), typeof(ObjectSelector), typeof(PlayerSelector), typeof(Reference), typeof(Union), typeof(SelfAbility),
    ];

    static readonly HashSet<Type> _quantities =
    [
        typeof(Fixed), typeof(Variable), typeof(CountOf), typeof(TotalOf), typeof(LifeTotal), typeof(DamageOrLifeTotal), typeof(Negated), typeof(Product), typeof(Arithmetic),
        typeof(Adjusted), typeof(Scaled), typeof(CharacteristicOf), typeof(Special), typeof(Bound), typeof(Extreme), typeof(DistinctCount), typeof(Devotion), typeof(AmountThisWay), typeof(AmountOfManaSpent),
    ];

    static readonly HashSet<Type> _conditions =
    [
        typeof(And), typeof(Not), typeof(Controls), typeof(CountCondition), typeof(CompareCount), typeof(ThereAre), typeof(IsKind), typeof(ObjectIs), typeof(AbilityResolvedNthTime),
        typeof(HasKeywordsCondition), typeof(HasCountersCondition), typeof(IsInZone), typeof(ItsPhase), typeof(Did), typeof(OutcomeCondition), typeof(Happened),
        typeof(WasCastWith), typeof(Targets), typeof(HandSize), typeof(WasAffectedThisWay), typeof(WasCast), typeof(ManaSpent), typeof(CompareControl), typeof(Or),
    ];

    static readonly HashSet<Type> _events =
    [
        typeof(ObjectEvent), typeof(PhaseEvent), typeof(SpellCastEvent), typeof(PlayerEvent), typeof(EitherEvent), typeof(PlayerAction), typeof(NamedEvent), typeof(EffectEvent), typeof(WouldHappen), typeof(CombatEncounterEvent), typeof(ConditionEvent),
    ];

    static readonly HashSet<Type> _costs =
    [
        typeof(PayMana), typeof(TapSelf), typeof(UntapSelf), typeof(PayLife), typeof(ActionCost), typeof(CompoundCost), typeof(CostChoice), typeof(KeywordCost),
    ];

    static readonly HashSet<Type> _predicates =
    [
        typeof(HasKeywords), typeof(HasStats), typeof(CharacteristicBound), typeof(CharacteristicCompared), typeof(HasExtreme), typeof(Distinct), typeof(Related), typeof(ActedOn), typeof(CastFrom), typeof(QualifierChoice), typeof(QualifiedKind),
        typeof(CastBy), typeof(WithCounters), typeof(Satisfying), typeof(ChosenBy), typeof(OfChosen), typeof(Targeting), typeof(SharesCharacteristic), typeof(Named), typeof(InCombatWith), typeof(RelativeCharacteristic), typeof(WithKeyword),
    ];

    static readonly HashSet<Type> _durations =
    [
        typeof(ThisTurn), typeof(UntilPhase), typeof(While), typeof(UntilThisLeaves), typeof(DuringPhase),
    ];

    static readonly HashSet<Type> _other =
    [
        typeof(Term), typeof(ObjectFilter), typeof(Qualifier), typeof(Controlled), typeof(ZoneRef), typeof(PhaseRef), typeof(DamagePortion),
        typeof(CounterSpec), typeof(TokenSpec), typeof(TokenCopy), typeof(Compared), typeof(KeywordInstance), typeof(Playable), typeof(StatsMod), typeof(ManaAmount), typeof(ManaChoice), typeof(Timing), typeof(PayCost),
    ];

    static readonly HashSet<string> _objectEvents =
    [
        "EntersTheBattlefield", "EntersOrTransforms", "EntersUntapped", "Dies", "IsPutIntoAGraveyardFromTheBattlefield", "IsPutIntoAGraveyardFromAnywhere",
        "IsPutIntoAGraveyard", "LeavesTheBattlefield", "LeavesTheBattlefieldWithoutDying", "IsPutIntoExileFromTheBattlefield", "Attacks", "AttacksOrBlocks",
        "AttacksAlone", "AttacksYou", "AttacksAPlayer", "AttacksAndIsntBlocked", "BecomesMonstrous", "Blocks", "BlocksOrBecomesBlocked", "BlocksACreature", "BecomesBlocked", "BecomesTapped",
        "BecomesUntapped", "DealsCombatDamageToAPlayer", "DealsCombatDamageToAPlayerOrPlaneswalker", "DealsCombatDamageToAnOpponent", "DealsCombatDamageToYou",
        "DealsCombatDamage", "DealsDamage", "DealsDamageToAPlayer", "DealsDamageToAnOpponent", "DealsDamageToACreature", "DealsDamageToYou",
        "DealsDamageToAPlayerOrPlaneswalker", "IsDealtDamage", "IsDealtCombatDamage", "Cast", "Discarded", "CountersPut", "Enters", "BecomesTarget", "Die", "LeaveTheBattlefield", "Countered", "IsTappedForMana", "IsTurnedFaceUp",
        "DealsCombatDamageToACreature", "LeaveYourGraveyard", "LeavesYourGraveyard",
    ];

    static readonly HashSet<string> _qualifierVocabularies =
    [
        "CardType", "CreatureType", "LandType", "PermanentSubtype", "PlaneswalkerType", "ArtifactType", "EnchantmentType", "Supertype", "ManaColor",
        "ColorCount", "Colorfulness", "TapState", "CombatState", nameof(ObjectKind), "Distinction", "Keyword", "AttachedBy", "Kind", "Position", "Provenance", "TokenWord", "CounterKind", "CardGroup", "CountSuffix", "Manner", "AbilitiesWord",
        "LeftoverCards", "AffectedThisWay", "DamageHistory", "ThisWayAction", "CastFrom", "SpellOrdinal", "SpellCountPeriod", "AbilityCategory",
    ];

    static readonly HashSet<string> _kindVocabularies =
    [
        nameof(ObjectKind), "TokenWord", "PlayerIdentity", "ManaColor", "TapState", "CombatState", "RetargetedObject", "CopiedSpell", "SourceNoun", "CardType", "CreatureType", "LandType", "PermanentSubtype", "PlaneswalkerType", "ArtifactType", "EnchantmentType", "SpellSubtype", "Supertype", "PermanentKind", nameof(ChosenCharacteristic), "TypeDesignation",
    ];

    /// <summary>
    /// A permanent's line that is an effect with no trigger or cost does something only as a static ability or where the
    /// engine reads it (as the permanent enters, a permission); otherwise it's lowered but never run.
    /// </summary>
    public static IEnumerable<string> OnPermanent(CardRules card, LineRules line)
    {
        if (card.Types?.Contains("Instant") == true || card.Types?.Contains("Sorcery") == true)
            yield break;
        if (line.Abilities is not [var first, ..] || CardDefinition.Unwrap(first) is not EffectLine effectLine)
            yield break;
        if (CardDefinition.AsStatic(effectLine) is StaticAbility statics)
        {
            if (!IsStaticHandled(Characteristics.WithoutPhase(statics.Effect)))
                yield return $"StaticAbility[{statics.Effect.GetType().Name}]";
            yield break;
        }
        var read = effectLine.Effect switch
        {
            Conditional { Then: EntersBattlefield, Else: null } => true,
            LetVariable { Effect: EntersBattlefield { Objects: null or Self, If: null, With.Count: > 0 } } => true,
            // "This spell costs {x} less to cast, where x is …": read where the spell's cost is (Game.CostToCast).
            CostModification { Spells: Self } or LetVariable { Effect: CostModification { Spells: Self } } => true,
            BecomeCopy { Verb: "EntersAs" } or Optional { Effect: BecomeCopy { Verb: "EntersAs" } } => true,
            CantBeTargeted { Objects: Self } => true,
            // Looking at hidden cards changes nothing in the game (the engine keeps no secrets from its agents).
            Optional { Effect: LookAt } or LookAt => true,
            _ => false,
        };
        if (!read)
            yield return $"EffectLine.OnPermanent[{Describe(effectLine.Effect)}]";
    }

    /// <summary>
    /// "Spend this mana only to …" is kept for the mana ability it follows, or the mana added just before it in an
    /// effect; elsewhere it isn't tied to any mana.
    /// </summary>
    public static IEnumerable<string> PoolRestrictions(LineRules line)
    {
        var abilities = line.Abilities.Select(CardDefinition.Unwrap).ToList();
        for (var i = 0; i < abilities.Count; i++)
        {
            var restrictions = MTGRulesIR.Lowering.NodeWalker.Nodes(abilities[i]).OfType<ManaRestriction>().ToList();
            if (restrictions.Count == 0)
                continue;
            // After a mana ability, or right after adding mana in a sequence ("add {r}{r}{r}. spend this mana only to …").
            var kept = abilities[i] is StaticAbility { Effect: ManaRestriction } or SpellAbility { Effect: ManaRestriction } && i > 0 && abilities[i - 1] is ActivatedAbility { Effect: AddMana or LetVariable { Effect: AddMana } } or SpellAbility { Effect: AddMana }
                || MTGRulesIR.Lowering.NodeWalker.Nodes(abilities[i]).OfType<Sequence>().Any(s => s.Steps.Zip(s.Steps.Skip(1)).Any(p => p.First is AddMana && p.Second is ManaRestriction))
                    && restrictions.Count == MTGRulesIR.Lowering.NodeWalker.Nodes(abilities[i]).OfType<Sequence>().Sum(s => s.Steps.Zip(s.Steps.Skip(1)).Count(p => p.First is AddMana && p.Second is ManaRestriction));
            if (!kept)
                yield return "ManaRestriction.PoolMana";
        }
    }

    static string Describe(Effect effect) =>
        effect switch
        {
            Optional optional => "Optional." + optional.Effect?.GetType().Name,
            Conditional conditional => "Conditional." + conditional.Then?.GetType().Name,
            LetVariable let => "LetVariable." + let.Effect?.GetType().Name,
            _ => effect?.GetType().Name,
        };

    /// <summary>The names of the nodes under <paramref name="root"/> (itself included) that the interpreter can't handle.</summary>
    public static IEnumerable<string> Unsupported(IrNode root)
    {
        // "If an opponent cast a red spell this turn, you may pay {w} rather than pay this spell's mana cost" is an alternative cost with a condition.
        if (root is Ability offering && CardDefinition.OfferedAlternative(offering) is { } offered)
            root = offered;
        var replaced = new HashSet<IrNode>(ReferenceEqualityComparer.Instance);
        foreach (var node in MTGRulesIR.Lowering.NodeWalker.Nodes(root))
            if (node is Replacement replacement && Game.DamageReplacement(replacement) is not null)
                replaced.Add(replacement.Instead is Sequence { Steps: [var first, ..] } ? first : replacement.Instead);
        // An event inside a history condition ("if a creature entered this turn") is read by History, not matched as a trigger.
        foreach (var node in MTGRulesIR.Lowering.NodeWalker.Nodes(root))
            if (node is Happened { Event: ObjectEvent historical } happened && HappenedHandled(happened))
                replaced.Add(historical);
        foreach (var node in MTGRulesIR.Lowering.NodeWalker.Nodes(root))
            if (!replaced.Contains(node) && Problem(node) is { } problem)
                yield return problem;
        // Damage modifiers ("deals double that damage instead") are read from permanents' abilities, not set up by effects.
        var standing = (root is Ability ability ? CardDefinition.Unwrap(ability) : null) switch
        {
            ReplacementAbility { Effect: Replacement stated } => stated,
            StaticAbility { Effect: Replacement stated } => stated,
            _ => null,
        };
        foreach (var node in MTGRulesIR.Lowering.NodeWalker.Nodes(root))
            if (node is Replacement modifier && !ReferenceEquals(modifier, standing) && Game.DamageReplacement(modifier) is null && Game.ZoneReplacementOf(modifier) is null && Game.DamageModifier(modifier) is not null)
                yield return "Replacement.DamageModifier.Temporary";
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
            case Token when type != typeof(TokenSpec) && type != typeof(TokenCopy) && type != typeof(ThoseTokens):
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
            Animate { Transforms: true } => "Animate.Transforms",
            ObjectSelector { Quantifier: Quantifier.Count, Count: null } => "ObjectSelector.Count",
            PlayerSelector { Kind: PlayerKind.Chosen or PlayerKind.EnchantedPlayer or PlayerKind.NoPlayer } player => $"PlayerSelector.{player.Kind}",
            Special { Kind: not ("ThatMany" or "ThatMuch" or "All" or "AnyNumber" or "OneOrMore" or "OneOrTwo") } special => $"Special.{special.Kind}",
            Variable { Name: not ("X" or "x") } variable => $"Variable.{variable.Name}",
            ObjectEvent objectEvent when objectEvent.Events.All(x => !_objectEvents.Contains(x)) => $"ObjectEvent.{objectEvent.Events.FirstOrDefault()}",
            ObjectEvent { By: not null } byEvent when byEvent.Events.Any(x => x != "BecomesTarget") || !Triggers.TargetedByKinds.Contains(byEvent.By) => "ObjectEvent.By",
            PhaseEvent phase when Conditions.StepOf(phase.At.Phase) is null => $"PhaseEvent.{phase.At.Phase}",
            TriggeredAbility { Trigger: PlayerAction { Action: "AttackedWith" or "Activate" } } => null,
            TriggeredAbility { Trigger: PlayerAction or NamedEvent { Kind: not "ClassBecomesLevel" } } trigger => $"TriggeredAbility[{trigger.Trigger.GetType().Name}]",
            Delayed { When: PlayerAction or NamedEvent } delayed => $"Delayed[{delayed.When.GetType().Name}]",
            PlayerEvent playerEvent when playerEvent.Event is not ("GainsLife" or "LosesLife" or "GainsOrLosesLife" or "Attacks" or "DiscardsACard" or "CyclesOrDiscardsACard" or "CyclesACard" or "CyclesThisCard" or "DrawsSecondCardEachTurn" or "SacrificesAnotherPermanent" or "ScriesOrSurveils" or "CommitsACrime") => $"PlayerEvent.{playerEvent.Event}",
            Qualifier { Vocabulary: "PermanentDesignation" } designation when !Filters.Designations.Contains(designation.Quality) => $"Qualifier[PermanentDesignation.{designation.Quality}]",
            Term { Vocabulary: "PermanentDesignation" } designation when !Filters.Designations.Contains(designation.Value) => $"Term[PermanentDesignation.{designation.Value}]",
            Qualifier qualifier when qualifier.Vocabulary != "PermanentDesignation" && !_qualifierVocabularies.Contains(qualifier.Vocabulary ?? "") => $"Qualifier[{qualifier.Vocabulary}]",
            Qualifier { Vocabulary: "CardGroup", Quality: not ("Historic" or "Outlaw") } group => $"Qualifier[CardGroup.{group.Quality}]",
            Qualifier { Vocabulary: "CountSuffix", Quality: not ("Other" or "OrMore") } suffix => $"Qualifier[CountSuffix.{suffix.Quality}]",
            Qualifier { Vocabulary: "Manner", Quality: not "AtRandom" } manner => $"Qualifier[Manner.{manner.Quality}]",
            Qualifier { Vocabulary: "Distinction", Quality: not (nameof(Distinction.Other) or nameof(Distinction.NamedThis) or nameof(Distinction.OrTied) or nameof(Distinction.NotOnTheBattlefield) or nameof(Distinction.BeyondTheFirst)) } distinction => $"Qualifier[Distinction.{distinction.Quality}]",
            Term term when term.Vocabulary != "PermanentDesignation" && !_kindVocabularies.Contains(term.Vocabulary ?? "") => $"Term[{term.Vocabulary}]",
            Related { Relation: not (Relation.OtherThan or Relation.AttachedTo or Relation.Among or Relation.Is or Relation.On or Relation.ExiledWith or Relation.AbilityOf or Relation.InPartyOf or Relation.DealtDamageBy) } related => $"Related.{related.Relation}",
            Happened happened when !HappenedHandled(happened) => $"Happened[{happened.Event?.GetType().Name}{(happened.Event is NamedEvent named ? "." + named.Kind : "")}]",
            PayCost { Cost: ActionCost { Action: var paid } } when !Game.PayableActions.Contains(paid.GetType()) => $"PayCost[ActionCost.{paid.GetType().Name}]",
            PayCost { Cost: not (PayMana or PayLife or ActionCost) } pay => $"PayCost[{pay.Cost?.GetType().Name}]",
            PayCost { Cost: PayMana mana } when mana.Symbols.Any(x => x.Contains('e', StringComparison.OrdinalIgnoreCase)) && !mana.Symbols.All(x => x.Equals("{e}", StringComparison.OrdinalIgnoreCase)) => "PayCost.Energy",
            Controlled { Relation: not ("Control" or "Own") } controlled => $"Controlled.{controlled.Relation}",
            ZoneRef zone when !Enum.TryParse<Zone>(zone.Zone, out _) => $"ZoneRef.{zone.Zone}",
            ZoneRef { Position: not (null or "Top" or "Bottom" or "TopOrBottom") } zone => $"ZoneRef.Position.{zone.Position}",
            PutCounters { Counters.Kind: null } => "PutCounters.Kind",
            ManaChoice { Choice: not ("AnyColor" or "AnyOneColor" or "AnyCombination" or "TheChosenColor" or "AnyTypeProduced") } choice => $"ManaChoice.{choice.Choice}",
            Destroy { Manner: not (null or "CantBeRegenerated") } destroy => $"Destroy.{destroy.Manner}",
            Exile { Until: not (null or UntilThisLeaves) } => "Exile.Until",
            CombatEncounterEvent encounter when !Triggers.EncounterKinds.Contains(encounter.Encounter) => $"CombatEncounterEvent.{encounter.Encounter}",
            RevealUntil { Until: not PlayerAction { What: ObjectSelector } } => "RevealUntil.Until",
            CantPlay { Verb: not ("Cast" or "Play") } cantPlay => $"CantPlay.{cantPlay.Verb}",
            CopySpell { Except: not null } => "CopySpell.Except",
            Union { Members.Count: 0 } => "Union.Empty",
            Phasing { Duration: not (null or UntilThisLeaves) } => "Phasing.Duration",
            AssignsNoCombatDamage { Duration: not (ThisTurn or UntilPhase { Phase.Phase: "EndOfTurn" }) } => "AssignsNoCombatDamage.Duration",
            ActedOn { Action: not ("Cast" or "Drawn" or "Discarded" or "CycledOrDiscarded" or "Sacrificed") } or ActedOn { Span: not ("ThisTurn" or null) } => "ActedOn",
            DistinctCount { Aspect: not ("Color" or "Colors" or "CardType" or "CardTypes" or "CreatureType" or "CreatureTypes" or "Name" or "Names" or "BasicLandTypes" or "BasicLandType" or "ManaValues" or "ManaValue" or "PermanentTypes") } distinct => $"DistinctCount.{distinct.Aspect}",
            DistributeCounters { Targeted: false } or DistributeCounters { Among: not ObjectSelector } => "DistributeCounters.Untargeted",
            ExtraLandPlays { EachTurn: false } oneTurn when !(oneTurn.Duration is null or ThisTurn && oneTurn.Count is Fixed) => "ExtraLandPlays.Duration",
            ExtraLandPlays { EachTurn: true, Count: not (Fixed or Special { Kind: "AnyNumber" }) } => "ExtraLandPlays.Count",
            GainControl { Duration: { } controlFor } when !Game.ControlDurationKept(controlFor) => $"GainControl.Duration[{Describe(controlFor)}]",
            BecomeCopy { Duration: { } duration } when !Keeps(duration) => $"BecomeCopy.Duration[{Describe(duration)}]",
            CastAsThoughFlash { Extra: not null } => "CastAsThoughFlash.Extra",
            Skip skip when skip.Event is not null || skip.Phase?.Phase != "Turn" && Game.SkippableStep(skip.Phase?.Phase) is null => $"Skip.{skip.Phase?.Phase ?? skip.Event}",
            ChangeTargets { To: not (null or Self) } => "ChangeTargets.To",
            CanBlock { Count: not null } => "CanBlock.Count",
            ManaRestriction restriction when restriction.Uses.Any(x => x.Verb is not ("Cast" or "Activate")) => $"ManaRestriction.{restriction.Uses.First(x => x.Verb is not ("Cast" or "Activate")).Verb}",
            CanAsThough asThough when Characteristics.AsThough(asThough) is null => $"CanAsThough.{asThough.Action}.{asThough.Keyword}",
            Remains remains when remains.State is not ("Tapped" or "Untapped" or "OnTheBattlefield" or "Exiled") => $"Remains.{remains.State}",
            GameOutcome { Result: not ("Win" or "Lose") } outcome => $"GameOutcome.{outcome.Result}",
            AmountOfManaSpent { Kind: { } spentKind } when Colors.OfName(spentKind) is null => $"AmountOfManaSpent.{spentKind}",
            AmountThisWay { Kind: "ExcessDamage" } => "AmountThisWay.ExcessDamage",
            CounterSpec { Prefix: "Nth" } => "CounterSpec.Nth",
            CantBeTargeted { ByQualifiers: not null } => "CantBeTargeted.ByQualifiers",
            PayCost { Times: not null } => "PayCost.Times",
            AlternativeCost { Spells: not null } => "AlternativeCost.Spells",
            // "{x}, {t}: draw a card. x is …": the engine still lets the player choose X.
            ActivatedAbility { Effect: LetVariable } defined when defined.Costs.Any(c => c is PayMana mana && mana.Symbols.Any(s => s.Equals("{x}", StringComparison.OrdinalIgnoreCase))) => "ActivatedAbility.DefinedX",
            AmountOfManaSpent { From: not null } => "AmountOfManaSpent.From",
            AssignsDamageAsThoughUnblocked { Duration: { } duration } when !Keeps(duration) => $"AssignsDamageAsThoughUnblocked.Duration[{Describe(duration)}]",
            WasCast { During: { } castDuring } when !Conditions.CastDuringSupported(castDuring) => $"WasCast.During.{castDuring.Phase}",
            // A second mode count under a condition ("choose both instead") isn't read by the modal rules yet.
            Instead { Effect: ChooseModes } => "ChooseModes.Instead",
            AssignsDamageByToughness { Duration: { } duration } when !Keeps(duration) => $"AssignsDamageByToughness.Duration[{Describe(duration)}]",
            GainAbilitiesOf { Duration: { } duration } when !Keeps(duration) => $"GainAbilitiesOf.Duration[{Describe(duration)}]",
            GainAbilitiesOf { Category: not "Activated" } gainedOf => $"GainAbilitiesOf.{gainedOf.Category}",
            NoMaximumHandSize { Duration: not null } => "NoMaximumHandSize.Duration",
            MaximumHandSize { RestOfGame: true } => "MaximumHandSize.RestOfGame",
            TriggeredAbility { If: { } condition } when Problem(condition) is { } problem => problem,
            KeywordAction action when action.Actions.Any(x => !KeywordActions.Supported.Contains(x.Keyword)) => $"KeywordAction.{action.Actions.First(x => !KeywordActions.Supported.Contains(x.Keyword)).Keyword}",
            KeywordAction { Other: { Count: > 0 } } => "KeywordAction.Other",
            UnlessPaid { Payment.Cost: ActionCost { Action: var unlessAction } } when !Game.PayableActions.Contains(unlessAction.GetType()) => $"UnlessPaid[ActionCost.{unlessAction.GetType().Name}]",
            UnlessPaid { Payment.Cost: not (PayMana or PayLife or ActionCost) } unless when KeywordAbilities.CostOf(unless.Payment.Cost) is not (PayMana or PayLife or ActionCost) => $"UnlessPaid[{unless.Payment.Cost?.GetType().Name}]",
            UnlessPaid { Payment.PerEach: not null, Payment.Cost: not PayMana } => "UnlessPaid.PerEach",
            CombatRestriction { Modifiers: { Count: > 0 } modifiers } when modifiers.Any(x => x is not ("Alone" or "Except")) => "CombatRestriction.Modifiers",
            CombatRestriction { By: not (null or Self or ObjectSelector) } => "CombatRestriction.By",
            CombatRestriction restriction when restriction.Cant.Any(x => x is not ("Attack" or "Block" or "BeBlocked")) => $"CombatRestriction.Cant.{restriction.Cant.First(x => x is not ("Attack" or "Block" or "BeBlocked"))}",
            CombatRestriction { Recipient: not (null or "You" or "YouOrYourPlaneswalkers") } restriction => $"CombatRestriction.Recipient.{restriction.Recipient}",
            PlayCard { Duration: { } duration } when !Keeps(duration) => $"PlayCard.Duration[{Describe(duration)}]",
            PlayCard { ByPaying: not null } => "PlayCard.ByPaying",
            PlayCard { From: not null } => "PlayCard.From",
            MustCombat must when must.Period is not (null or "EachCombat" or "ThisTurn") || must.Actions.Any(x => x is not ("Attack" or "Block")) => "MustCombat.Kind",
            CantBeTargeted cantBe when cantBe.By?.Any(x => x is not ("Countered" or "Spells" or "SpellsOrAbilities")) == true => $"CantBeTargeted.{cantBe.By.First(x => x is not ("Countered" or "Spells" or "SpellsOrAbilities"))}",
            CantBeTargeted { Duration: { } duration } when !Keeps(duration) => $"CantBeTargeted.Duration[{Describe(duration)}]",
            EffectEvent { Effect: not (Draw or Sacrifice or PlayCard { Card: ObjectSelector }) } effectEvent => $"EffectEvent.{effectEvent.Effect?.GetType().Name}",
            CombatRestriction { Duration: { } duration } when !Keeps(duration) => $"CombatRestriction.Duration[{Describe(duration)}]",
            SearchLibrary { Zones: not (null or "Library" or "LibraryAndOrGraveyard" or "LibraryAndGraveyard" or "GraveyardHandAndLibrary" or "GraveyardHandAndOrLibrary") } search => $"SearchLibrary.{search.Zones}",
            EntersBattlefield enters when enters.States?.Any(x => x is not ("Tapped" or "Escapes")) == true => $"EntersBattlefield.{enters.States.First(x => x is not ("Tapped" or "Escapes"))}",
            EntersBattlefield { Duration: not null } => "EntersBattlefield.Duration",
            ReplacementAbility { Effect: not (EntersBattlefield or AsEventHappens or Replacement) } replacement => $"ReplacementAbility[{replacement.Effect.GetType().Name}]",
            Replacement replacement when Game.DamageReplacement(replacement) is null && Game.ZoneReplacementOf(replacement) is null && Game.DamageModifier(replacement) is null => $"Replacement[{replacement.Would?.GetType().Name}{(replacement.Would is WouldHappen { What: var what } ? "." + what?.GetType().Name : "")}]",
            Replacement { Duration: { } duration } when !Keeps(duration) => $"Replacement.Duration[{Describe(duration)}]",
            WouldHappen { What: not DealDamage } would => $"WouldHappen.{would.What?.GetType().Name}",
            Qualifier { Vocabulary: "DamageHistory", Quality: not ("DealtDamageThisWay" or "DealtDamageThisTurn") } history => $"Qualifier[DamageHistory.{history.Quality}]",
            ReplacementAbility { Effect: AsEventHappens { When: not ObjectEvent } } => "AsEventHappens.When",
            AsEventHappens { When: ObjectEvent happens } when happens.Events.All(x => x is not ("EntersTheBattlefield" or "EntersOrTransforms" or "Enters")) => $"AsEventHappens.{happens.Events.FirstOrDefault()}",
            AdditionalCost { Spells: not Self } => "AdditionalCost.Spells",
            AlternativeCost { Replaces: not "ThisSpellsManaCost" } alternative => $"AlternativeCost.{alternative.Replaces}",
            Convoke { Spells: not (null or Self) } => "Convoke.Spells",
            ChooseEffect { Kinds: [var kind, ..] } when kind != "Opponent" && Choices.Category(kind) is not ("Color" or "CreatureType" or "CardName" or "LandType" or "CardType" or "OddOrEven") => $"ChooseEffect.{kind}",
            ChooseEffect { Kinds: null or [], From: not [PlayerSelector or ObjectSelector, ..] } => "ChooseEffect.From",
            OfChosen chosen when !Choices.Supported.Contains(chosen.Characteristic) => $"OfChosen.{chosen.Characteristic}",
            SharesCharacteristic shares when shares.Characteristic is not ("Name" or "Color" or "CreatureType" or "CardType") => $"SharesCharacteristic.{shares.Characteristic}",
            BecomesAspect { Negated: true } negated when negated.Aspects.Any(x => x.Vocabulary != "CardType") => "BecomesAspect.Negated",
            BecomesAspect { Duration: { } duration } when !Keeps(duration) => $"BecomesAspect.Duration[{Describe(duration)}]",
            BecomesAspect aspect when aspect.Aspects.FirstOrDefault(x => !AspectHandled(x)) is { } unhandled => $"BecomesAspect[{unhandled.Vocabulary}]",
            PreventDamage { Amount: Special { Kind: "ThatMuch" } } => "PreventDamage.ThatMuch",
            RedirectDamage { Amount: Special { Kind: not "All" } } => "RedirectDamage.Amount",
            MayPlayFrom from when !Game.PlayFromHandled(from) => "MayPlayFrom",
            DamageOrLifeTotal total when !(total is { Unit: "Damage", Change: "Prevented", When: "ThisWay" } || total is { When: "ThisTurn", Player: not null } && (total.Unit == "Life" && total.Change is "Gained" or "Lost" || total.Unit == "Damage" && total.Change == "Dealt")) => $"DamageOrLifeTotal.{total.Unit}{total.Change}{total.When}",
            RedirectDamage { Duration: { } redirectFor } when !Keeps(redirectFor) => $"RedirectDamage.Duration[{Describe(redirectFor)}]",
            PreventDamage { Duration: { } duration } when !Keeps(duration) => $"PreventDamage.Duration[{Describe(duration)}]",
            CostModification { Action: "Activate", Spells: SelfAbility } => null,
            CostModification { Action: not (null or "Cast") } modification => $"CostModification.{modification.Action}",
            WasCastWith { Twice: true } => "WasCastWith.Twice",
            WasCastWith cast when cast.How is not ("Kicked" or "AdditionalCost") => $"WasCastWith.{cast.How}",
            Targets { Spell: not (Self or Reference { Text: "it" }) } => "Targets.Spell",
            MustBlock { Duration: not (null or ThisTurn or UntilPhase { Phase.Phase: "EndOfTurn" }) } => "MustBlock.Duration",
            MustBeBlocked { Duration: not (ThisTurn or UntilPhase { Phase.Phase: "EndOfTurn" }) } => "MustBeBlocked.Duration",
            LabeledAbility { Word: "MaxSpeed", Ability: not (StaticAbility or TriggeredAbility or ActivatedAbility or EffectLine) } => "LabeledAbility.MaxSpeed",
            KeywordCost keywordCost when !KeywordAbilities.CostHandled(keywordCost) => "KeywordCost",
            CastBy cast when cast.Relation is not ("Control" or "Cast") || cast.Relation == "Cast" && (cast.ThisTurn || cast.From is not null || cast.During is not null) => $"CastBy.{cast.Relation}{(cast.ThisTurn ? ".ThisTurn" : "")}{(cast.From is not null ? ".From" : "")}",
            Timing timing when !Game.TimingKinds.Contains(timing.Kind) => $"Timing.{timing.Kind}",
            PayCost { Plus: not null } => "PayCost.Plus",
            Mode { Cost: not (null or PayMana) } => "Mode.Cost",
            ActivatedAbility activated when activated.Costs.Any(x => x is ActionCost { Action: var action } && !Game.PayableActions.Contains(action.GetType())) => "ActivatedAbility.Costs",
            KeywordLine line when line.Other is { Count: > 0 } && !line.Other.All(KeywordAbilities.OtherHandled) => "KeywordLine.Other",
            KeywordLine line when line.Keywords.FirstOrDefault(x => !Keywords.Handles(x.Keyword)) is { } keyword => $"Keyword.{CardDefinition.Normalize(keyword.Keyword)}",
            KeywordLine line when line.Keywords.Any(x => CardDefinition.Normalize(x.Keyword) == "Suspend" && !Game.Suspendable(x)) => "Keyword.Suspend.X",
            KeywordLine line when line.Keywords.Any(x => CardDefinition.Normalize(x.Keyword) is "Vanishing" or "Fading" && x.Amount is not Fixed { Value: > 0 }) => "Keyword.Vanishing.Amount",
            KeywordLine line when line.Keywords.Any(x => CardDefinition.Normalize(x.Keyword) == "Foretell" && x.Cost is not PayMana) => "Keyword.Foretell.Cost",
            KeywordLine line when line.Keywords.Any(x => CardDefinition.Normalize(x.Keyword) == "Typecycling" && string.IsNullOrEmpty(x.Parameter)) => "Keyword.Typecycling.Type",
            StaticAbility statics when !IsStaticHandled(Characteristics.WithoutPhase(statics.Effect)) => $"StaticAbility[{statics.Effect.GetType().Name}]",
            _ => null,
        };
    }

    static bool HappenedHandled(Happened happened) =>
        History.Supported(happened) || happened switch
        {
            { Event: NamedEvent { Kind: "YouAttacked" }, Span: "ThisTurn" or null } => true,
            { Event: PlayerAction { Action: "Cast", What: null or ObjectSelector { Filter: null or { Qualifiers: null, With: null } } }, Span: "ThisTurn" or null } => true,
            { Event: PlayerAction { Action: "GainedLife" or "LostLife" } } => true,
            _ => false,
        };

    static bool IsStaticHandled(Effect effect) =>
        effect switch
        {
            Pump { Duration: null } => true,
            CombatRestriction { Duration: null } => true,
            MustCombat { Period: null or "EachCombat" } => true,
            CantBeTargeted { Duration: null } => true,
            SetStats => true,
            CantPlay => true,
            CastAsThoughFlash { Extra: null } => true,
            MayNotUntap => true,
            CanBlock { Only: not null, Count: null } => true,
            CantBeActivated => true,
            Convoke { Spells: null or Self } => true,
            CanAsThough { Duration: null } => true,
            GrantAbility => true,
            LoseAllAbilities => true,
            DoesntUntap { If: null } => true,
            // Read with the ability before it, or as a condition on the static (CardDefinition.LastingUntaps).
            DoesntUntap { If: Did { Effect: Remains { Objects: Self, State: "Tapped" or "OnTheBattlefield" } } or Controls { Objects: Self } or HasCountersCondition } => true,
            MustBlock { Duration: null } => true,
            BecomesAspect { Duration: null } => true,
            Animate { Duration: null, Transforms: false } => true,
            PreventDamage { Duration: null, Amount: Special { Kind: "All" } or Fixed } => true,
            MayPlayFrom from => Game.PlayFromHandled(from),
            Replacement replacement => Game.DamageModifier(replacement) is not null || Game.DamageReplacement(replacement) is not null,
            CostModification { Spells: Self or ObjectSelector } => true,
            CostModification { Spells: SelfAbility, Action: "Activate" } => true,
            ManaRestriction => true,
            GainControl { Duration: null } => true,
            ExtraLandPlays { EachTurn: true, Duration: null, Count: Fixed or Special { Kind: "AnyNumber" } } => true,
            AssignsDamageByToughness { Duration: null } => true,
            AssignsDamageAsThoughUnblocked { Duration: null } => true,
            PlayWithHandRevealed => true,
            MaximumHandSize { RestOfGame: false } => true,
            NoMaximumHandSize { Duration: null } => true,
            GainAbilitiesOf { Duration: null, Category: "Activated" } => true,
            ProtectionDoesntRemoveThis => true,
            // Read at the untap step (Game): "untap all creatures you control during each other player's untap step".
            Untap { During: DuringPhase { Phase.Phase: "UntapStep" or "Untap" } } => true,
            Sequence sequence => sequence.Steps.All(x => IsStaticHandled(Characteristics.WithoutPhase(x))),
            _ => false,
        };

    static bool AspectHandled(Term aspect) =>
        aspect.Vocabulary switch
        {
            "CardType" or "CreatureType" or "LandType" or "ArtifactType" or "EnchantmentType" or "PermanentSubtype" or "ManaColor" => true,
            "TypeDesignation" => aspect.Value is "ChosenType" or "ChosenColor",
            "PermanentDesignation" => aspect.Value == "Saddled",
            nameof(ChosenCharacteristic) => aspect.Value is "ColorOfYourChoice" or "ColorOrColorsOfYourChoice" or "BasicLandTypeOfYourChoice" or "CreatureTypeOfYourChoice",
            _ => false,
        };

    static bool Keeps(Duration duration) =>
        duration switch
        {
            ThisTurn => true,
            UntilPhase { Phase.Phase: "EndOfTurn" or "EndStep" or "Cleanup" or "End" } => true,
            UntilPhase { Phase.Phase: "Turn", Phase.Next: true } => true,
            UntilPhase { Phase.Phase: "Untap" or "Upkeep" or "YourNextTurn" } => true,
            While or UntilThisLeaves => true,
            DuringPhase => true,
            _ => false,
        };

    static string Describe(Duration duration) =>
        duration is UntilPhase until ? $"UntilPhase.{until.Phase.Phase}" : duration.GetType().Name;
}
