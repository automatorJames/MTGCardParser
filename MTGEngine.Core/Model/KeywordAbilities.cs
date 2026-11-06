namespace MTGEngine.Core;

/// <summary>An activated ability that works from a zone other than the battlefield: cycling from the hand, unearth from the graveyard.</summary>
public sealed record ZoneAbility(Zone Zone, ActivatedAbility Ability) : Ability;

/// <summary>Warp's end: the permanent is exiled, and its owner may cast it from exile on a later turn.</summary>
public sealed record ExileForLater : Effect;

/// <summary>Abilities <see cref="Objects"/> gain until end of turn (backup's "it gains the following abilities"), unless it's this object itself.</summary>
public sealed record GrantUntilEndOfTurn(Selector Objects, IReadOnlyList<Ability> Abilities) : Effect;

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
    public static readonly ImmutableHashSet<string> Actions = ["Scry", "Surveil", "Investigate", "Proliferate", "Fateseal", "Populate", "Bolster", "Explore", "Adapt"];

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
            case "Affinity" when AffinityKind(keyword.Parameter) is { } kind:
                yield return new StaticAbility(new CostModification(new Self(), new PayMana(["{1}"]), "Less", "Cast",
                    new CountOf(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [kind], Controller: new Controlled(You))))));
                break;
            case var action when spell && Actions.Contains(action):
                yield return new SpellAbility(new KeywordAction([keyword]));
                break;
        }
    }

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
