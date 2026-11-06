namespace MTGEngine.Core;

/// <summary>An activated ability that works from a zone other than the battlefield: cycling from the hand, unearth from the graveyard.</summary>
public sealed record ZoneAbility(Zone Zone, ActivatedAbility Ability) : Ability;

/// <summary>Warp's end: the permanent is exiled, and its owner may cast it from exile on a later turn.</summary>
public sealed record ExileForLater : Effect;

/// <summary>Abilities <see cref="Objects"/> gain until end of turn (backup's "it gains the following abilities"), unless it's this object itself.</summary>
public sealed record GrantUntilEndOfTurn(Selector Objects, IReadOnlyList<Ability> Abilities) : Effect;

/// <summary>"Max speed —": the ability works while its controller's speed is 4.</summary>
public sealed record AtMaxSpeed : Condition;

/// <summary>Sunburst's count: the colors of mana spent to cast the object.</summary>
public sealed record ColorsSpentToCast : Quantity;

/// <summary>"For as long as this artifact remains tapped": the source is tapped (read without its characteristics, which may depend on it).</summary>
public sealed record SourceTapped : Condition;

/// <summary>Ninjutsu's effect: this card from the hand onto the battlefield tapped and attacking.</summary>
public sealed record NinjutsuEnter : Effect;

/// <summary>Embalm's and eternalize's token: a copy of this card, except it's <see cref="Color"/>, a zombie in addition to its other types, with no mana cost, and (eternalize) <see cref="Size"/>/<see cref="Size"/>.</summary>
public sealed record ZombieCopyOfThis(string Color, int? Size = null) : Effect;

/// <summary>"Its +1/+1 counters" (modular): the counters of <see cref="Kind"/> on this object, as it last was on the battlefield once it has left.</summary>
public sealed record CountersItHad(string Kind) : Quantity;

/// <summary>Evolve's condition: the creature that entered has greater power or greater toughness than this creature.</summary>
public sealed record Evolves : Condition;

/// <summary>Saddle's effect, and "it becomes saddled until end of turn": the permanent is saddled this turn.</summary>
public sealed record BecomeSaddled(Selector Objects = null) : Effect;

/// <summary>A creature cast for its sneak cost enters attacking (and tapped).</summary>
public sealed record EnterAttacking : Effect;

/// <summary>Rampage's count: the creatures blocking this one beyond the first.</summary>
public sealed record BlockersBeyondFirst : Quantity;

/// <summary>Collect evidence N: exiling cards from your graveyard with total mana value N or more.</summary>
public sealed record CollectEvidence(int Total) : Effect;

/// <summary>Discarding the card whose ability this is (cycling).</summary>
public sealed record DiscardThis : Cost;

/// <summary>Tapping untapped creatures you control with total power N or more (crew).</summary>
public sealed record TapCreaturesWithPower(int Power) : Cost;

/// <summary>
/// What keywords stand for, as IR the engine already runs: equip is an activated ability, prowess a trigger,
/// affinity a cost change, ward a "counter it unless" trigger. Keywords that are rules the engine reads where they
/// apply (flying, protection, landwalk, wither, persist) need nothing here; see <see cref="Keywords"/>.
/// </summary>
public static class KeywordAbilities
{
    static readonly PlayerSelector You = new(PlayerKind.You);
    static readonly Duration EndOfTurn = new UntilPhase(new PhaseRef("EndOfTurn"));
    static ObjectSelector Creatures(ObjectFilter filter = null) => new(Quantifier.Any, filter ?? new ObjectFilter(Kinds: [new Term("Creature", "CardType")]));

    static readonly ImmutableHashSet<string> _basicLandTypes = ["Plains", "Island", "Swamp", "Mountain", "Forest"];

    /// <summary>What a typecycling keyword searches for: "basic land" a basic land card, "plains" a Plains card, "sliver" a Sliver card.</summary>
    static ObjectFilter CycledFilter(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return null;
        var library = new ZoneRef(nameof(Zone.Library));
        if (type == "basic land")
            return new ObjectFilter(Qualifiers: [new Qualifier("Basic", Vocabulary: "Supertype"), new Qualifier("Land", Vocabulary: "CardType")], Zone: library);
        var name = char.ToUpperInvariant(type[0]) + type[1..];
        return new ObjectFilter(Kinds: [new Term(name, _basicLandTypes.Contains(name) ? "LandType" : "CreatureType")], Zone: library);
    }

    /// <summary>The keyword actions a spell's keyword line does as it resolves ("scry 1.").</summary>
    public static readonly ImmutableHashSet<string> Actions = ["Scry", "Surveil", "Investigate", "Proliferate", "Fateseal", "Populate", "Bolster", "Explore", "Adapt", "Learn", "Clash", "Incubate"];

    /// <summary>The abilities each keyword line stands for, after the line; a spell's keyword actions as part of the spell.</summary>
    public static IEnumerable<Ability> Expand(IEnumerable<Ability> abilities, bool spell)
    {
        var list = abilities.ToList();
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i] is not KeywordLine line)
            {
                yield return list[i];
                continue;
            }
            yield return line;
            foreach (var keyword in line.Keywords)
                foreach (var expanded in Of(keyword, spell, list.Skip(i + 1).ToList()))
                    yield return expanded;
            foreach (var other in line.Other ?? [])
                if (Rampage(other) is { } rampage)
                    yield return rampage;
        }
    }

    static IEnumerable<Ability> Of(KeywordInstance keyword, bool spell, IReadOnlyList<Ability> following)
    {
        var name = CardDefinition.Normalize(keyword.Keyword);
        var amount = keyword.Amount is Fixed { Value: var n } ? n : 1;
        switch (name)
        {
            case "Equip" when keyword.Cost is { } cost:
                yield return new ActivatedAbility([cost], new Attach(new Self(), new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Controller: new Controlled(You)))));
                yield return new ActivationLimits([new Timing(nameof(TimingKind.SorcerySpeed))]);
                break;
            case "Cycling" when keyword.Cost is { } cost:
                yield return new ZoneAbility(Zone.Hand, new ActivatedAbility([cost, new DiscardThis()], new Draw(You, new Fixed(1))));
                break;
            case "Typecycling" when keyword.Cost is { } cost && CycledFilter(keyword.Parameter) is { } wanted:
                yield return new ZoneAbility(Zone.Hand, new ActivatedAbility([cost, new DiscardThis()], new SearchLibrary(You, nameof(Zone.Library),
                    [new ObjectSelector(Quantifier.Count, wanted, new Fixed(1))], Reveal: true,
                    Placement: new ChangeZone(new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [new Term(nameof(ObjectKind.Card), nameof(ObjectKind))], Qualifiers: [new Qualifier("Found", Vocabulary: "Provenance")])), new ZoneRef(nameof(Zone.Hand))),
                    Ending: new Shuffle())));
                break;
            case "Unearth" when keyword.Cost is { } cost:
                yield return new ZoneAbility(Zone.Graveyard, new ActivatedAbility([cost], new Sequence([
                    new ChangeZone(new Self(), new ZoneRef(nameof(Zone.Battlefield))),
                    new Pump(new Self(), Keywords: [new KeywordInstance("Haste")]),
                    new Replacement(new ObjectEvent(new Self(), ["LeaveTheBattlefield"]), new Exile(new Self())),
                    new Delayed(new PhaseEvent(new PhaseRef("EndStep", Next: true)), new Exile(new Self())),
                ])));
                yield return new ActivationLimits([new Timing(nameof(TimingKind.SorcerySpeed))]);
                break;
            case "LevelUp" when keyword.Cost is { } cost:
                yield return new ActivatedAbility([cost], new PutCounters(new Self(), new CounterSpec("level", new Fixed(1))));
                yield return new ActivationLimits([new Timing(nameof(TimingKind.SorcerySpeed))]);
                break;
            case "Crew":
                yield return new ActivatedAbility([new TapCreaturesWithPower(amount)], new Animate(new Self(), Types: ["Creature"], InAddition: true, Duration: new ThisTurn()));
                break;
            case "Prowess":
                yield return new TriggeredAbility(
                    new SpellCastEvent(You, new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Spell", nameof(ObjectKind))], Qualifiers: [new Qualifier("Creature", true, "CardType")]))),
                    new Pump(new Self(), new Fixed(1), new Fixed(1), Duration: EndOfTurn));
                break;
            case "Exalted":
                yield return new TriggeredAbility(
                    new ObjectEvent(Creatures(new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Controller: new Controlled(You))), ["AttacksAlone"]),
                    new Pump(new Reference(null, "it"), new Fixed(1), new Fixed(1), Duration: EndOfTurn));
                break;
            case "Flanking":
                yield return new TriggeredAbility(
                    new CombatEncounterEvent(new Self(), "BecomesBlockedBy", Creatures(new ObjectFilter(Kinds: [new Term("Creature", "CardType")], With: [new HasKeywords(["Flanking"], true)]))),
                    new Pump(new Reference(null, "that creature"), new Fixed(-1), new Fixed(-1), Duration: EndOfTurn));
                break;
            case "Bushido":
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["BlocksOrBecomesBlocked"]), new Pump(new Self(), new Fixed(amount), new Fixed(amount), Duration: EndOfTurn));
                break;
            case "Bloodthirst":
                yield return new ReplacementAbility(new EntersBattlefield(new Self(), With: [new PutCounters(null, new CounterSpec("+1/+1", new Fixed(amount)))],
                    If: new Happened(new PlayerAction(new PlayerSelector(PlayerKind.AnyOpponent), "WasDealtDamage"), "ThisTurn")));
                break;
            case "Ward" when keyword.Cost is { } cost:
            {
                var thatSpell = new Reference(null, "that spell");
                yield return new TriggeredAbility(
                    new ObjectEvent(new Self(), ["BecomesTarget"], By: "SpellOrAbility", ByController: new Controlled(new PlayerSelector(PlayerKind.YourOpponents))),
                    new UnlessPaid(new CounterSpell(thatSpell), new PayCost(cost, new PlayerSelector(PlayerKind.ControllerOf, thatSpell))));
                break;
            }
            case "Echo" when keyword.Cost is { } cost:
                yield return new TriggeredAbility(
                    new PhaseEvent(new PhaseRef("Upkeep", Whose: You)),
                    new Sequence([new UnlessPaid(new Sacrifice(You, new Self()), new PayCost(cost)), new PutCounters(new Self(), new CounterSpec("echo-paid", new Fixed(1)))]),
                    new HasCountersCondition(new Self(), new CounterSpec("echo-paid", null), Negated: true));
                break;
            // Vanishing N: enters with N time counters; each upkeep one comes off, and when the last does it's sacrificed.
            case "Vanishing" when keyword.Amount is Fixed { Value: > 0 }:
                yield return new ReplacementAbility(new EntersBattlefield(new Self(), With: [new PutCounters(new Self(), new CounterSpec("time", new Fixed(amount)))]));
                yield return new TriggeredAbility(
                    new PhaseEvent(new PhaseRef("Upkeep", Whose: You)),
                    new Sequence([
                        new RemoveCounters(new Self(), new CounterSpec("time", new Fixed(1))),
                        new Conditional(new HasCountersCondition(new Self(), new CounterSpec("time", null), Negated: true), new Sacrifice(You, new Self())),
                    ]),
                    new HasCountersCondition(new Self(), new CounterSpec("time", null)));
                break;
            // Fading N: enters with N fade counters; each upkeep one comes off, or it's sacrificed if none can.
            case "Fading" when keyword.Amount is Fixed { Value: > 0 }:
                yield return new ReplacementAbility(new EntersBattlefield(new Self(), With: [new PutCounters(new Self(), new CounterSpec("fade", new Fixed(amount)))]));
                yield return new TriggeredAbility(
                    new PhaseEvent(new PhaseRef("Upkeep", Whose: You)),
                    new Conditional(new HasCountersCondition(new Self(), new CounterSpec("fade", null)),
                        new RemoveCounters(new Self(), new CounterSpec("fade", new Fixed(1))),
                        new Sacrifice(You, new Self())));
                break;
            case "CumulativeUpkeep" when keyword.Cost is { } cost:
            {
                var age = new CountOf(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Counter", nameof(ObjectKind))], Qualifiers: [new Qualifier("age", Vocabulary: "CounterKind")], With: [new Related(Relation.On, new Self())])));
                yield return new TriggeredAbility(
                    new PhaseEvent(new PhaseRef("Upkeep", Whose: You)),
                    new Sequence([new PutCounters(new Self(), new CounterSpec("age", new Fixed(1))), new UnlessPaid(new Sacrifice(You, new Self()), new PayCost(cost, PerEach: age))]));
                break;
            }
            // Soulshift N: when it dies, you may return a spirit card with mana value N or less from your graveyard to your hand.
            case "Soulshift" when keyword.Amount is Fixed:
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["Dies"]), new Optional(new ChangeZone(
                    new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [new Term(nameof(ObjectKind.Card), nameof(ObjectKind))], Qualifiers: [new Qualifier("Spirit", Vocabulary: "CreatureType")],
                        With: [new CharacteristicBound(["ManaValue"], new Fixed(amount), Comparison.LessOrEqual)], Zone: new ZoneRef(nameof(Zone.Graveyard), You))),
                    new ZoneRef(nameof(Zone.Hand), You))));
                break;
            // Fabricate N: when it enters, N +1/+1 counters on it or N 1/1 colorless servo artifact creature tokens.
            case "Fabricate" when keyword.Amount is Fixed:
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["EntersTheBattlefield"]), new ChoiceOf([
                    new PutCounters(new Self(), new CounterSpec("+1/+1", new Fixed(amount))),
                    new CreateTokens(new TokenSpec(Types: ["Artifact", "Creature"], Subtypes: ["Servo"], Power: new Fixed(1), Toughness: new Fixed(1)), new Fixed(amount)),
                ]));
                break;
            // Renown N: when it deals combat damage to a player, if it isn't renowned, N +1/+1 counters and it becomes renowned.
            case "Renown" when keyword.Amount is Fixed:
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["DealsCombatDamageToAPlayer"]),
                    new Sequence([new PutCounters(new Self(), new CounterSpec("+1/+1", new Fixed(amount))), new PutCounters(new Self(), new CounterSpec("renowned", new Fixed(1)))]),
                    new HasCountersCondition(new Self(), new CounterSpec("renowned", null), Negated: true));
                break;
            // Sunburst: it enters with a +1/+1 counter (a charge counter if it isn't a creature) for each color of mana spent to cast it.
            case "Sunburst" when !spell:
                yield return new ReplacementAbility(new EntersBattlefield(new Self(), With: [new PutCounters(new Self(), new CounterSpec("sunburst", new ColorsSpentToCast()))]));
                break;
            // Ninjutsu: {cost}, return an unblocked attacker you control to its owner's hand: put this card onto the battlefield tapped and attacking (once blockers are declared; see Game).
            case "Ninjutsu" when keyword.Cost is { } cost:
                yield return new ZoneAbility(Zone.Hand, new ActivatedAbility([cost, new ActionCost(new ChangeZone(
                    new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Qualifiers: [new Qualifier("Unblocked", Vocabulary: "CombatState")], Controller: new Controlled(You))),
                    new ZoneRef(nameof(Zone.Hand))))], new NinjutsuEnter()));
                break;
            // Offspring: paid as it's cast (see Game), a 1/1 token copy of it as it enters.
            case "Offspring" when keyword.Cost is not null:
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["EntersTheBattlefield"]),
                    new CreateTokens(new TokenCopy(new Self(), Stats: new StatsMod(new Fixed(1), new Fixed(1))), new Fixed(1)), new WasCastWith(new Self(), "Offspring"));
                break;
            // Warp: cast for the warp cost (see Game), exiled at the next end step, castable from exile on a later turn.
            case "Warp" when keyword.Cost is not null:
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["EntersTheBattlefield"]),
                    new Delayed(new PhaseEvent(new PhaseRef("EndStep", Next: true)), new ExileForLater()), new WasCastWith(new Self(), "Warped"));
                break;
            // Backup N: as it enters, N +1/+1 counters on target creature, which (if another) gains the abilities below until end of turn.
            case "Backup" when keyword.Amount is Fixed:
            {
                var backed = new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [new Term("Creature", "CardType")]), Binding: "#backup");
                var it = new Reference("#backup", "it");
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["EntersTheBattlefield"]), new Sequence([
                    new PutCounters(backed, new CounterSpec("+1/+1", new Fixed(amount))),
                    new GrantUntilEndOfTurn(it, following.Where(x => x is not (EffectLine or SpellAbility)).ToList()),
                ]));
                break;
            }
            // Awaken N: cast for the awaken cost (see Game), it also puts N +1/+1 counters on target land you control, which becomes a 0/0 elemental creature with haste.
            case "Awaken" when spell && keyword.Cost is not null && keyword.Amount is Fixed:
            {
                var land = new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [new Term("Land", "CardType")], Controller: new Controlled(You)), Binding: "#awaken");
                var it = new Reference("#awaken", "it");
                yield return new SpellAbility(new Conditional(new WasCastWith(new Self(), "Awakened"), new Sequence([
                    new PutCounters(land, new CounterSpec("+1/+1", new Fixed(amount))),
                    new Animate(it, new Fixed(0), new Fixed(0), Subtypes: ["Elemental"], Types: ["Creature"], Keywords: [new KeywordInstance("Haste")], InAddition: true),
                ])));
                break;
            }
            // Evoke: cast for the evoke cost (see Game), it's sacrificed when it enters.
            case "Evoke" when keyword.Cost is not null:
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["EntersTheBattlefield"]), new Sacrifice(You, new Self()), new WasCastWith(new Self(), "Evoked"));
                break;
            // Dash: cast for the dash cost (see Game), it has haste and returns to its owner's hand at the next end step.
            case "Dash" when keyword.Cost is not null:
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["EntersTheBattlefield"]),
                    new Delayed(new PhaseEvent(new PhaseRef("EndStep", Next: true)), new ChangeZone(new Self(), new ZoneRef(nameof(Zone.Hand)))), new WasCastWith(new Self(), "Dashed"));
                break;
            // Scavenge: {cost}, exile this card from your graveyard: +1/+1 counters equal to its power on target creature, as a sorcery.
            case "Scavenge" when keyword.Cost is { } cost:
                yield return new ZoneAbility(Zone.Graveyard, new ActivatedAbility([cost, new ActionCost(new Exile(new Self()))],
                    new PutCounters(new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [new Term("Creature", "CardType")])), new CounterSpec("+1/+1", new CharacteristicOf(new Self(), "Power")))));
                yield return new ActivationLimits([new Timing(nameof(TimingKind.SorcerySpeed))]);
                break;
            // Embalm and eternalize: {cost}, exile this card from your graveyard: a token copy of it, a white (or black 4/4) zombie with no mana cost, as a sorcery.
            case "Embalm" or "Eternalize" when keyword.Cost is { } cost:
                yield return new ZoneAbility(Zone.Graveyard, new ActivatedAbility([cost, new ActionCost(new Exile(new Self()))],
                    name == "Embalm" ? new ZombieCopyOfThis("White") : new ZombieCopyOfThis("Black", 4)));
                yield return new ActivationLimits([new Timing(nameof(TimingKind.SorcerySpeed))]);
                break;
            // Transmute: {cost}, discard this card: search your library for a card with the same mana value, reveal it, put it into your hand, as a sorcery.
            case "Transmute" when keyword.Cost is { } cost:
                yield return new ZoneAbility(Zone.Hand, new ActivatedAbility([cost, new DiscardThis()], new SearchLibrary(You, nameof(Zone.Library),
                    [new ObjectSelector(Quantifier.Count, new ObjectFilter(Kinds: [new Term(nameof(ObjectKind.Card), nameof(ObjectKind))], With: [new CharacteristicBound(["ManaValue"], new CharacteristicOf(new Self(), "ManaValue"), Comparison.Equal)], Zone: new ZoneRef(nameof(Zone.Library))), new Fixed(1))], Reveal: true,
                    Placement: new ChangeZone(new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [new Term(nameof(ObjectKind.Card), nameof(ObjectKind))], Qualifiers: [new Qualifier("Found", Vocabulary: "Provenance")])), new ZoneRef(nameof(Zone.Hand))),
                    Ending: new Shuffle())));
                yield return new ActivationLimits([new Timing(nameof(TimingKind.SorcerySpeed))]);
                break;
            // Modular N: enters with N +1/+1 counters; when it dies, you may put its +1/+1 counters on target artifact creature.
            case "Modular" when keyword.Amount is Fixed:
                yield return new ReplacementAbility(new EntersBattlefield(new Self(), With: [new PutCounters(new Self(), new CounterSpec("+1/+1", new Fixed(amount)))]));
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["Dies"]), new Optional(new PutCounters(
                    new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Qualifiers: [new Qualifier("Artifact", Vocabulary: "CardType")])),
                    new CounterSpec("+1/+1", new CountersItHad("+1/+1")))));
                break;
            // Unleash: you may have it enter with a +1/+1 counter; it can't block as long as it has one.
            case "Unleash":
                yield return new ReplacementAbility(new EntersBattlefield(new Self(), With: [new Optional(new PutCounters(new Self(), new CounterSpec("+1/+1", new Fixed(1))))]));
                yield return new StaticAbility(new CombatRestriction(new Self(), ["Block"], If: new HasCountersCondition(new Self(), new CounterSpec("+1/+1", null))));
                break;
            // Mentor: whenever it attacks, a +1/+1 counter on target attacking creature with lesser power.
            case "Mentor":
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["Attacks"]), new PutCounters(
                    new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Qualifiers: [new Qualifier("Attacking", Vocabulary: "CombatState")],
                        With: [new CharacteristicBound(["Power"], new CharacteristicOf(new Self(), "Power"), Comparison.Less)])),
                    new CounterSpec("+1/+1", new Fixed(1))));
                break;
            // Evolve: whenever another creature you control enters, if it has greater power or toughness than this creature, a +1/+1 counter on this creature.
            case "Evolve":
                yield return new TriggeredAbility(
                    new ObjectEvent(new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Controller: new Controlled(You)), Other: true), ["EntersTheBattlefield"]),
                    new PutCounters(new Self(), new CounterSpec("+1/+1", new Fixed(1))), new Evolves());
                break;
            // Extort: whenever you cast a spell, you may pay {W/B}; if you do, each opponent loses 1 life and you gain that much life.
            case "Extort":
                yield return new TriggeredAbility(
                    new SpellCastEvent(You, new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Spell", nameof(ObjectKind))]))),
                    new Sequence([
                        new Optional(new PayCost(new PayMana(["{w/b}"]))),
                        new Conditional(new OutcomeCondition(You, "Did"), new Sequence([new LoseLife(new PlayerSelector(PlayerKind.EachOpponent), new Fixed(1)), new GainLife(You, new Special("ThatMuch"))])),
                    ]));
                break;
            // Saddle N: tap other untapped creatures you control with total power N or more: it becomes saddled until end of turn, as a sorcery.
            case "Saddle":
                yield return new ActivatedAbility([new TapCreaturesWithPower(amount)], new BecomeSaddled());
                yield return new ActivationLimits([new Timing(nameof(TimingKind.SorcerySpeed))]);
                break;
            // Station: tap another creature you control: charge counters equal to its power on this permanent, as a sorcery.
            case "Station":
                yield return new ActivatedAbility([new ActionCost(new Tap(new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Controller: new Controlled(You)), Other: true)))],
                    new PutCounters(new Self(), new CounterSpec("charge", new CharacteristicOf(new ObjectSelector(Quantifier.Those, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Qualifiers: [new Qualifier("Tapped", Vocabulary: "ThisWayAction")])), "Power"))));
                yield return new ActivationLimits([new Timing(nameof(TimingKind.SorcerySpeed))]);
                break;
            // Riot: it enters with a +1/+1 counter or with haste (haste matters only the turn it enters).
            case "Riot":
                yield return new ReplacementAbility(new EntersBattlefield(new Self(), With: [new ChoiceOf([
                    new PutCounters(new Self(), new CounterSpec("+1/+1", new Fixed(1))),
                    new Pump(new Self(), Keywords: [new KeywordInstance("Haste")], Duration: EndOfTurn),
                ])]));
                break;
            // Afflict N: whenever it becomes blocked, defending player loses N life.
            case "Afflict" when keyword.Amount is Fixed:
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["BecomesBlocked"]), new LoseLife(new PlayerSelector(PlayerKind.DefendingPlayer), new Fixed(amount)));
                break;
            // Annihilator N: whenever it attacks, defending player sacrifices N permanents.
            case "Annihilator" when keyword.Amount is Fixed:
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["Attacks"]), new Sacrifice(new PlayerSelector(PlayerKind.DefendingPlayer),
                    new ObjectSelector(Quantifier.Count, new ObjectFilter(Kinds: [new Term("Permanent", "CardType")]), new Fixed(amount))));
                break;
            // Outlast {cost}: {cost}, {T}: a +1/+1 counter on it, as a sorcery.
            case "Outlast" when keyword.Cost is { } cost:
                yield return new ActivatedAbility([cost, new TapSelf()], new PutCounters(new Self(), new CounterSpec("+1/+1", new Fixed(1))));
                yield return new ActivationLimits([new Timing(nameof(TimingKind.SorcerySpeed))]);
                break;
            // Decayed: it can't block; when it attacks, it's sacrificed at end of combat.
            case "Decayed":
                yield return new StaticAbility(new CombatRestriction(new Self(), ["Block"]));
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["Attacks"]), new Delayed(new PhaseEvent(new PhaseRef("EndOfCombat")), new Sacrifice(You, new Self())));
                break;
            // Reinforce N, {cost}: {cost}, discard this card: N +1/+1 counters on target creature.
            case "Reinforce" when keyword.Cost is { } cost && keyword.Amount is Fixed:
                yield return new ZoneAbility(Zone.Hand, new ActivatedAbility([cost, new DiscardThis()],
                    new PutCounters(new ObjectSelector(Quantifier.Target, new ObjectFilter(Kinds: [new Term("Creature", "CardType")])), new CounterSpec("+1/+1", new Fixed(amount)))));
                break;
            // Ingest: whenever it deals combat damage to a player, that player exiles the top card of their library.
            case "Ingest":
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["DealsCombatDamageToAPlayer"]), new Exile(new ObjectSelector(Quantifier.Count,
                    new ObjectFilter(Kinds: [new Term(nameof(ObjectKind.Card), nameof(ObjectKind))], Qualifiers: [new Qualifier("Top", Vocabulary: "Position")], Zone: new ZoneRef(nameof(Zone.Library), new PlayerSelector(PlayerKind.ThatPlayer))), new Fixed(1))));
                break;
            // Blitz: cast for the blitz cost (see Game), it has haste, draws a card when it dies, and is sacrificed at the next end step.
            case "Blitz" when keyword.Cost is not null:
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["EntersTheBattlefield"]),
                    new Delayed(new PhaseEvent(new PhaseRef("EndStep", Next: true)), new Sacrifice(You, new Self())), new WasCastWith(new Self(), "Blitz"));
                yield return new TriggeredAbility(new ObjectEvent(new Self(), ["Dies"]), new Draw(You, new Fixed(1)), new WasCastWith(new Self(), "Blitz"));
                break;
            // Graft N: enters with N +1/+1 counters; whenever another creature enters, you may move a +1/+1 counter from this onto it.
            case "Graft" when keyword.Amount is Fixed:
                yield return new ReplacementAbility(new EntersBattlefield(new Self(), With: [new PutCounters(new Self(), new CounterSpec("+1/+1", new Fixed(amount)))]));
                yield return new TriggeredAbility(
                    new ObjectEvent(new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Creature", "CardType")]), Other: true), ["EntersTheBattlefield"]),
                    new Optional(new MoveCounters(new CounterSpec("+1/+1", new Fixed(1)), new Self(), new Reference(null, "it"))),
                    new HasCountersCondition(new Self(), new CounterSpec("+1/+1", null)));
                break;
            case "Affinity" when AffinityKind(keyword.Parameter) is { } kind:
                yield return new StaticAbility(new CostModification(new Self(), new PayMana(["{1}"]), "Less", "Cast",
                    new CountOf(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [kind], Controller: new Controlled(You))))));
                break;
            case var action when spell && Actions.Contains(action):
                yield return new SpellAbility(new KeywordAction([keyword]));
                break;
        }
    }

    /// <summary>Rampage N, which the lowering leaves as text: whenever it becomes blocked, +N/+N until end of turn for each creature blocking it beyond the first.</summary>
    static TriggeredAbility Rampage(IrNode other) =>
        other is OpaqueNode { Glyph: "Rampage", Text: var text } && int.TryParse(text.Split(' ').LastOrDefault(), out var n)
            ? new TriggeredAbility(new ObjectEvent(new Self(), ["BecomesBlocked"]), new Pump(new Self(), new Fixed(n), new Fixed(n), PerEach: new BlockersBeyondFirst(), Duration: EndOfTurn))
            : null;

    /// <summary>Whether the engine carries out a keyword line's entry the lowering left as text.</summary>
    public static bool OtherHandled(IrNode other) => Rampage(other) is not null;

    /// <summary>A keyword cost as a cost the engine pays: "blight 1" puts a -1/-1 counter on a creature you control, "collect evidence 3" exiles cards from your graveyard. Any other cost as it is.</summary>
    public static Cost CostOf(Cost cost) =>
        cost switch
        {
            KeywordCost { Keyword: { Keyword: "Blight", Amount: Fixed { Value: var blight } } } =>
                new ActionCost(new PutCounters(new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Controller: new Controlled(You))), new CounterSpec("-1/-1", new Fixed(blight)))),
            KeywordCost { Keyword: { Keyword: "CollectEvidence", Amount: Fixed { Value: var evidence } } } => new ActionCost(new CollectEvidence(evidence)),
            _ => cost,
        };

    /// <summary>The keyword costs <see cref="CostOf"/> turns into costs the engine pays.</summary>
    public static bool CostHandled(KeywordCost cost) => CostOf(cost) is not KeywordCost;

    /// <summary>The kind "affinity for artifacts / equipment / forests" counts, read from the keyword's parameter.</summary>
    static Term AffinityKind(string parameter)
    {
        if (parameter is null)
            return null;
        var at = parameter.IndexOf("affinity for ", StringComparison.OrdinalIgnoreCase);
        if (at < 0)
            return null;
        var word = new string(parameter[(at + "affinity for ".Length)..].TakeWhile(char.IsLetter).ToArray());
        var single = word.EndsWith("ies") ? word[..^3] + "y" : word.EndsWith('s') ? word[..^1] : word;
        var value = CardDefinition.Normalize(single);
        return value switch
        {
            "Artifact" or "Creature" or "Enchantment" or "Land" or "Planeswalker" => new Term(value, "CardType"),
            "Plains" or "Island" or "Swamp" or "Mountain" or "Forest" => new Term(value, "LandType"),
            "Equipment" => new Term(value, "ArtifactType"),
            _ => new Term(value, "CreatureType"),
        };
    }

    /// <summary>A keyword's names in a characteristics' keyword set: the keyword, and for protection or toxic its parameter ("ProtectionFromRed", "Toxic:2").</summary>
    public static IEnumerable<string> NamesOf(KeywordInstance keyword)
    {
        var name = CardDefinition.Normalize(keyword.Keyword);
        yield return name;
        if (name == "Protection" && keyword.Parameter is { } from)
            foreach (var part in from.Split([" and from ", " and ", ", from ", ","], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                yield return "ProtectionFrom" + CardDefinition.Normalize(part.Replace("from ", ""));
        if (name == "Toxic" && keyword.Amount is Fixed { Value: var poison })
            yield return $"Toxic:{poison}";
    }
}
