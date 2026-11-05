namespace MTGEngine.Core;

/// <summary>An activated ability that works from a zone other than the battlefield: cycling from the hand, unearth from the graveyard.</summary>
public sealed record ZoneAbility(Zone Zone, ActivatedAbility Ability) : Ability;

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

    /// <summary>The keyword actions a spell's keyword line does as it resolves ("scry 1.").</summary>
    public static readonly ImmutableHashSet<string> Actions = ["Scry", "Surveil", "Investigate", "Proliferate", "Fateseal", "Populate", "Bolster", "Explore", "Adapt"];

    /// <summary>The abilities each keyword line stands for, after the line; a spell's keyword actions as part of the spell.</summary>
    public static IEnumerable<Ability> Expand(IEnumerable<Ability> abilities, bool spell)
    {
        foreach (var ability in abilities)
        {
            if (ability is not KeywordLine line)
            {
                yield return ability;
                continue;
            }
            yield return line;
            foreach (var keyword in line.Keywords)
                foreach (var expanded in Of(keyword, spell))
                    yield return expanded;
        }
    }

    static IEnumerable<Ability> Of(KeywordInstance keyword, bool spell)
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
            case "CumulativeUpkeep" when keyword.Cost is { } cost:
            {
                var age = new CountOf(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Counter", nameof(ObjectKind))], Qualifiers: [new Qualifier("age", Vocabulary: "CounterKind")], With: [new Related(Relation.On, new Self())])));
                yield return new TriggeredAbility(
                    new PhaseEvent(new PhaseRef("Upkeep", Whose: You)),
                    new Sequence([new PutCounters(new Self(), new CounterSpec("age", new Fixed(1))), new UnlessPaid(new Sacrifice(You, new Self()), new PayCost(cost, PerEach: age))]));
                break;
            }
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
