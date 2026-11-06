namespace MTGEngine.Core;

/// <summary>
/// An object's characteristics as they are now: printed values, then counters, temporary effects, and the static
/// abilities of permanents on the battlefield. A light version of the layer system: type, subtype and color changes
/// first, then abilities, then power and toughness - enough for anthems, pumps, +1/+1 counters, keyword grants,
/// granted abilities, combat restrictions, losing all abilities, animation and "nonbasic lands are mountains".
/// </summary>
public sealed record Characteristics(
    int Power,
    int Toughness,
    ImmutableHashSet<string> Keywords,
    ImmutableHashSet<string> Types,
    ImmutableHashSet<string> Subtypes,
    ImmutableHashSet<Color> Colors,
    bool LostAbilities,
    ImmutableHashSet<string> Restrictions,
    ImmutableList<Ability> Abilities,
    ImmutableList<CombatRule> CombatRules = null)
{
    public ImmutableList<CombatRule> Rules => CombatRules ?? ImmutableList<CombatRule>.Empty;

    public bool HasKeyword(string keyword) => Keywords.Contains(CardDefinition.Normalize(keyword));

    public bool IsCreature => Types.Contains("Creature");

    /// <summary>Whether a combat action is denied: "Attack", "Block", "BeBlocked".</summary>
    public bool Cant(string action) => Restrictions.Contains(action);

    [ThreadStatic] static int _judgingCondition;

    static readonly ImmutableHashSet<string> _landTypes =
        ["Plains", "Island", "Swamp", "Mountain", "Forest", "Desert", "Gate", "Cave", "Lair", "Locus", "Mine", "Power-Plant", "Tower", "Sphere", "Town", "Urza's"];

    public static bool IsLandType(string subtype) => _landTypes.Contains(subtype);

    /// <summary>
    /// The allowance a "can … as though" grants, as a restriction word the combat rules read: attacking despite
    /// defender or as though it had haste, blocking creatures with shadow. Being blockable as though a creature didn't
    /// have landwalk is what the engine already does, so it grants nothing. Null for one the engine doesn't know.
    /// </summary>
    public static string AsThough(CanAsThough asThough) =>
        (asThough.Action, asThough.Keyword) switch
        {
            ("Attack", "Defender") => "AttackDespiteDefender",
            ("Attack", "Haste") => "AttackAsThoughHaste",
            ("Block", "Shadow") => "BlockShadow",
            ("BeBlocked", { } walk) when walk.EndsWith("walk") => "Landwalk",
            _ => null,
        };

    /// <summary>Runs a condition judged inside characteristics (a lasting effect's "for as long as"), where nested characteristics leave statics out so it can't recurse.</summary>
    public static T Judging<T>(Func<T> judge)
    {
        _judgingCondition++;
        try { return judge(); }
        finally { _judgingCondition--; }
    }

    /// <summary>Characteristics already worked out for a state: states are immutable, so an answer holds for as long as the state does.</summary>
    static readonly System.Runtime.CompilerServices.ConditionalWeakTable<GameState, System.Collections.Concurrent.ConcurrentDictionary<(GameObject, bool), Characteristics>> _cache = new();

    public static Characteristics Of(GameState state, GameObject obj, bool withStatics = true)
    {
        // A static's condition is judged on characteristics without statics, so conditions can't recurse into themselves.
        withStatics &= _judgingCondition == 0;
        // Only the answers judged outside a condition are kept: inside one, the same question is asked with statics left out.
        if (_judgingCondition > 0)
            return Compute(state, obj, withStatics);
        var known = _cache.GetOrCreateValue(state);
        if (known.TryGetValue((obj, withStatics), out var cached))
            return cached;
        var computed = Compute(state, obj, withStatics);
        known[(obj, withStatics)] = computed;
        return computed;
    }

    static readonly System.Runtime.CompilerServices.ConditionalWeakTable<CardDefinition, CardDefinition> _faceDown = new();
    static readonly System.Runtime.CompilerServices.ConditionalWeakTable<CardDefinition, CardDefinition> _prototyped = new();

    /// <summary>A card's prototype: "prototype {1}{w} — 2/2" as its mana cost, the colors of that cost, and that power and toughness.</summary>
    static CardDefinition Prototyped(CardDefinition card)
    {
        if (card.Abilities.OfType<KeywordLine>().SelectMany(x => x.Keywords).FirstOrDefault(x => x.Keyword == "Prototype") is not { Cost: PayMana cost } prototype)
            return card;
        var mana = ManaCost.Parse(string.Concat(cost.Symbols));
        return card with
        {
            ManaCost = mana,
            Colors = mana.Colored.Keys.Concat(mana.Hybrid.SelectMany(x => x)).ToImmutableHashSet(),
            Power = prototype.Stats?.Power is Fixed { Value: var power } ? power : card.Power,
            Toughness = prototype.Stats?.Toughness is Fixed { Value: var toughness } ? toughness : card.Toughness,
        };
    }

    static Characteristics Compute(GameState state, GameObject obj, bool withStatics)
    {
        // A face-down spell or permanent is a nameless, colorless 2/2 creature with no abilities.
        if (obj.FaceDown && obj.Zone is Zone.Battlefield or Zone.Stack)
            obj = obj with { Card = _faceDown.GetValue(obj.Card, Game.FaceDownCard) };
        // Cast for its prototype cost, it has the prototype's mana cost, color, power and toughness.
        else if (obj.WasCastWith("Prototyped") && obj.Zone is Zone.Battlefield or Zone.Stack)
            obj = obj with { Card = _prototyped.GetValue(obj.Card, Prototyped) };
        var card = obj.Card;
        var power = card.Power ?? 0;
        var toughness = card.Toughness ?? 0;
        var keywords = card.Keywords.ToBuilder();
        // A creature cast as its last time counter was removed has haste, and one cast for its dash cost.
        if (obj.Zone == Zone.Battlefield && (obj.WasCastWith("Suspended") || obj.WasCastWith("Dashed") || obj.WasCastWith("Blitz")))
            keywords.Add("Haste");
        var restrictions = ImmutableHashSet.CreateBuilder<string>();
        var rules = ImmutableList.CreateBuilder<CombatRule>();
        var abilities = card.Abilities.ToBuilder();
        var types = card.Types;
        var subtypes = card.Subtypes;
        // Bestowed and attached, it's an aura, not a creature; when what it enchants leaves, it's a creature again.
        if (obj.Zone == Zone.Battlefield && obj.AttachedTo is not null && obj.WasCastWith("Bestowed"))
        {
            types = types.Remove("Creature");
            subtypes = subtypes.Add("Aura");
        }
        var colors = card.Colors;
        var lost = false;

        var temporary = state.Temporary.Where(x => x.Target == obj.Id && x.ActiveIn(state)).ToList();
        var statics = withStatics ? StaticEffects(state).Where(x => Applies(state, x.Objects, x.Source, obj)).ToList() : [];

        // Types, subtypes and colors (layers 4 and 5): statics, then effects for a while in the order they began.
        var loseLandAbilities = false;
        void Change(TypeChange change)
        {
            if (change is null)
                return;
            if (change.Types is { Count: > 0 })
                types = change.InAddition ? types.Union(change.Types) : change.Types.ToImmutableHashSet();
            if (change.Subtypes is { Count: > 0 })
            {
                if (change.InAddition)
                    subtypes = subtypes.Union(change.Subtypes);
                else if (change.Subtypes.All(IsLandType) && types.Contains("Land"))
                {
                    // A land given a land type loses its other land types and the abilities of its rules text (305.7).
                    subtypes = subtypes.Where(x => !IsLandType(x)).ToImmutableHashSet().Union(change.Subtypes);
                    loseLandAbilities = true;
                }
                else
                    subtypes = change.Subtypes.ToImmutableHashSet();
            }
            if (change.Colors is { } newColors)
                colors = change.InAddition ? colors.Union(newColors) : newColors.ToImmutableHashSet();
            if (change.Removes is { Count: > 0 } removed)
                types = types.Except(removed);
        }
        foreach (var (effect, _, source) in statics)
            Change(TypeChangeOf(effect, source));
        foreach (var effect in temporary)
            Change(effect.TypeChange);
        if (loseLandAbilities)
            foreach (var printed in card.Abilities)
                abilities.Remove(printed);

        foreach (var effect in temporary)
        {
            if (effect.BaseStats is { } stats)
            {
                if (stats.Power is Fixed p) power = p.Value;
                if (stats.Toughness is Fixed t) toughness = t.Value;
            }
            if (effect.LosesAbilities)
            {
                lost = true;
                keywords.Clear();
                abilities.Clear();
            }
        }

        foreach (var (effect, _, source) in statics)
        {
            if (effect is SetStats set)
            {
                // A characteristic-defining ability: its count is judged without statics, so it can't count itself into a loop.
                int? value;
                _judgingCondition++;
                try { value = Quantities.Evaluate(set.Amount, EffectContext.Snapshot(state, source.Controller, source.Id)); }
                finally { _judgingCondition--; }
                if (value is { } amount)
                {
                    if (set.Which is "Power" or "PowerAndToughness")
                        power = amount;
                    if (set.Which is "Toughness" or "PowerAndToughness")
                        toughness = amount;
                }
                continue;
            }
            var stats = effect switch
            {
                Pump { BaseStats: { } pumped } => pumped,
                Animate { BaseStats: { } animated } => animated,
                Animate { Power: not null } or Animate { Toughness: not null } => new StatsMod(((Animate)effect).Power, ((Animate)effect).Toughness),
                _ => null,
            };
            if (stats is not null)
            {
                if (stats.Power is Fixed p) power = p.Value;
                if (stats.Toughness is Fixed t) toughness = t.Value;
            }
        }

        power += obj.CountersOf("+1/+1") - obj.CountersOf("-1/-1");
        toughness += obj.CountersOf("+1/+1") - obj.CountersOf("-1/-1");

        foreach (var effect in temporary)
        {
            power += effect.Power;
            toughness += effect.Toughness;
            foreach (var keyword in effect.Keywords)
                keywords.Add(CardDefinition.Normalize(keyword));
            foreach (var restriction in effect.Restrictions ?? [])
                restrictions.Add(restriction);
            rules.AddRange(effect.CombatRules ?? []);
            foreach (var ability in effect.Abilities ?? [])
            {
                abilities.Add(ability);
                if (ability is KeywordLine line)
                {
                    foreach (var keyword in line.Keywords)
                        keywords.UnionWith(KeywordAbilities.NamesOf(keyword));
                    abilities.AddRange(KeywordAbilities.Expand([line], false).Skip(1));
                }
            }
        }

        foreach (var (effect, _, source) in statics)
            switch (effect)
            {
                case Pump pump:
                {
                    var factor = pump.Remove ? -1 : 1;
                    // "Gets +1/+1 for each other creature you control": counted as the source sees it, without statics
                    // (as a characteristic-defining count is), so a count can't feed on itself.
                    if (pump.PerEach is not null)
                    {
                        _judgingCondition++;
                        // "Its" in the count is the pumped object ("enchanted creature gets -x/-x, where x is the number of creature cards in its controller's graveyard").
                        try { factor *= Quantities.Evaluate(pump.PerEach, EffectContext.Snapshot(state, source.Controller, source.Id) with { TriggerSubject = obj.Id == source.Id ? null : TargetRef.Of(obj.Id) }) ?? 0; }
                        finally { _judgingCondition--; }
                    }
                    power += factor * ((pump.Power as Fixed)?.Value ?? 0);
                    toughness += factor * ((pump.Toughness as Fixed)?.Value ?? 0);
                    foreach (var keyword in pump.Keywords ?? [])
                        foreach (var named in KeywordAbilities.NamesOf(keyword))
                            if (pump.Remove)
                                keywords.Remove(named);
                            else
                                keywords.Add(named);
                    break;
                }
                case Animate animate:
                    foreach (var keyword in animate.Keywords ?? [])
                        keywords.UnionWith(KeywordAbilities.NamesOf(keyword));
                    break;
                case CombatRestriction { By: null, If: null, Modifiers: null or { Count: 0 }, Recipient: null } restriction:
                    foreach (var cant in restriction.Cant)
                        restrictions.Add(cant);
                    break;
                case CombatRestriction restriction:
                    foreach (var cant in restriction.Cant)
                        rules.Add(new CombatRule(cant, restriction.By, restriction.If, restriction.Modifiers, restriction.Recipient, source.Id, source.Controller));
                    break;
                case CanAsThough asThough when AsThough(asThough) is { } allowance:
                    restrictions.Add(allowance);
                    break;
                case CanBlock { Only: { } only, Count: null }:
                    rules.Add(new CombatRule("BlockOnly", only, null, null, null, source.Id, source.Controller));
                    break;
                case CantBeActivated cantActivate:
                    restrictions.Add(cantActivate.ExceptMana ? "ActivateNonMana" : "Activate");
                    break;
                case MustCombat must:
                    foreach (var action in must.Actions)
                        restrictions.Add("Must" + action);
                    break;
                case CantBeTargeted cantBe when cantBe.By?.Any(x => x is "Spells" or "SpellsOrAbilities") == true:
                    restrictions.Add(cantBe.Controller is not null ? "BeTargetedByOpponents" : "BeTargeted");
                    break;
                case GrantAbility grant:
                    abilities.Add(grant.Ability);
                    if (grant.Ability is KeywordLine line)
                    {
                        foreach (var keyword in line.Keywords)
                            keywords.UnionWith(KeywordAbilities.NamesOf(keyword));
                        abilities.AddRange(KeywordAbilities.Expand([line], false).Skip(1));
                    }
                    foreach (var keyword in grant.Alongside ?? [])
                        keywords.UnionWith(KeywordAbilities.NamesOf(keyword));
                    break;
                case LoseAllAbilities:
                    lost = true;
                    keywords.Clear();
                    abilities.Clear();
                    break;
            }

        return new Characteristics(power, toughness, keywords.ToImmutable(), types, subtypes, colors, lost, restrictions.ToImmutable(), abilities.ToImmutable(), rules.ToImmutable());
    }

    /// <summary>The abilities an object has now: printed, granted for a while, and granted by statics on the battlefield.</summary>
    public static ImmutableList<Ability> AbilitiesOf(GameState state, GameObject obj) => Of(state, obj).Abilities;

    /// <summary>The type, subtype and color change a static effect makes, read for <paramref name="source"/> (whose choices "the chosen type" means); null for none.</summary>
    public static TypeChange TypeChangeOf(Effect effect, GameObject source) =>
        effect switch
        {
            BecomesAspect { Negated: true } aspect => new TypeChange(Removes: aspect.Aspects.Where(x => x.Vocabulary == "CardType").Select(x => x.Value).ToImmutableList()),
            BecomesAspect aspect => TypeChange.OfAspects(aspect.Aspects, aspect.InAddition, source),
            Animate animate => TypeChange.OfAnimate(animate),
            _ => null,
        };

    /// <summary>The continuous effects of static abilities on the battlefield whose condition (if any) holds now: each effect with what it applies to and its source.</summary>
    public static IEnumerable<(Effect Effect, Selector Objects, GameObject Source)> StaticEffects(GameState state)
    {
        foreach (var source in state.Battlefield.Concat(state.Emblems))
        {
            if (state.Temporary.Any(x => x.Target == source.Id && x.LosesAbilities))
                continue;
            if (source.FaceDown)
                continue;
            // Printed and temporarily granted statics; statics granted by other statics would recurse, so only those.
            var abilities = source.Card.Abilities.Concat(state.Temporary.Where(x => x.Target == source.Id && x.ActiveIn(state)).SelectMany(x => x.Abilities ?? []));
            foreach (var ability in abilities.OfType<StaticAbility>())
            {
                if (ability.While is { } condition)
                {
                    bool? holds;
                    _judgingCondition++;
                    try { holds = Conditions.Evaluate(condition, EffectContext.Snapshot(state, source.Controller, source.Id)); }
                    finally { _judgingCondition--; }
                    if (holds != true)
                        continue;
                }
                foreach (var effect in Continuous(During(ability.Effect, state, source)))
                    if (ObjectsOf(effect) is { } objects)
                        yield return (effect, objects, source);
            }
        }
    }

    /// <summary>A static effect "during your turn" as the effect without its phase when it's that phase now, nothing when it isn't.</summary>
    static Effect During(Effect effect, GameState state, GameObject source)
    {
        if (effect is Sequence sequence)
            return new Sequence(sequence.Steps.Select(x => During(x, state, source)).Where(x => x is not null).ToList());
        var property = effect.GetType().GetProperty("Duration");
        if (property?.GetValue(effect) is not DuringPhase during)
            return effect;
        if (!PhaseHolds(during.Phase, state, source))
            return null;
        var copy = (Effect)effect.GetType().GetMethod("<Clone>$")!.Invoke(effect, null);
        property.SetValue(copy, null);
        return copy;
    }

    /// <summary>Whether it's the named phase of the named player's turn now, read from <paramref name="source"/>'s controller's side.</summary>
    public static bool PhaseHolds(PhaseRef phase, GameState state, GameObject source)
    {
        var whose = phase.Whose switch
        {
            null => true,
            PlayerSelector { Kind: PlayerKind.You } => state.ActivePlayer == source.Controller,
            PlayerSelector { Kind: PlayerKind.EachOpponent or PlayerKind.AnyOpponent or PlayerKind.YourOpponents } => state.ActivePlayer != source.Controller,
            _ => true,
        };
        return whose && phase.Phase switch
        {
            null or "Turn" => true,
            "CombatPhase" or "Combat" => state.Step is Step.BeginCombat or Step.DeclareAttackers or Step.DeclareBlockers or Step.CombatDamage or Step.EndCombat,
            var named => Conditions.StepOf(named) == state.Step,
        };
    }

    /// <summary>
    /// Whether <paramref name="effect"/>, stated by a permanent's line without a trigger or cost ("creatures you control
    /// get +1/+1", "this creature attacks each combat if able", "during your turn, …"), is a continuous effect the static
    /// rules apply - so the line is a static ability.
    /// </summary>
    public static bool IsContinuous(Effect effect) =>
        effect switch
        {
            Sequence sequence => sequence.Steps.Count > 0 && sequence.Steps.All(IsContinuous),
            _ => WithoutPhase(effect) is { } plain && Continuous(plain).Any() && ObjectsOf(plain) is not null,
        };

    /// <summary>The effect without a "during your turn" duration (applied by <see cref="During"/>), or itself.</summary>
    public static Effect WithoutPhase(Effect effect)
    {
        var property = effect.GetType().GetProperty("Duration");
        if (property?.GetValue(effect) is not DuringPhase)
            return effect;
        var copy = (Effect)effect.GetType().GetMethod("<Clone>$")!.Invoke(effect, null);
        property.SetValue(copy, null);
        return copy;
    }

    static IEnumerable<Effect> Continuous(Effect effect) =>
        effect switch
        {
            null => [],
            Pump { Duration: null } => [effect],
            CombatRestriction { Duration: null } => [effect],
            MustCombat { Period: null or "EachCombat" } => [effect],
            CantBeTargeted { Duration: null } => [effect],
            SetStats => [effect],
            CanAsThough { Duration: null } => [effect],
            CanBlock { Only: not null, Count: null } => [effect],
            CantBeActivated => [effect],
            GrantAbility => [effect],
            LoseAllAbilities => [effect],
            BecomesAspect { Duration: null } => [effect],
            Animate { Duration: null, Transforms: false } => [effect],
            PreventDamage { Duration: null } => [effect],
            MayPlayFrom { Duration: null } => [effect],
            GainControl { Duration: null } => [effect],
            ExtraLandPlays { EachTurn: true, Duration: null } => [effect],
            DoesntUntap { If: null } => [effect],
            MustBlock { Duration: null } => [effect],
            Sequence sequence => sequence.Steps.SelectMany(Continuous),
            _ => [],
        };

    static Selector ObjectsOf(Effect effect) =>
        effect switch
        {
            Pump pump => pump.Objects ?? new Self(),
            CombatRestriction restriction => restriction.Objects ?? new Self(),
            GrantAbility grant => grant.Objects ?? new Self(),
            LoseAllAbilities lose => lose.Objects ?? new Self(),
            BecomesAspect aspect => aspect.Objects ?? new Self(),
            MustCombat must => must.Objects ?? new Self(),
            CantBeTargeted cantBe => cantBe.Objects ?? new Self(),
            SetStats set => set.Objects ?? new Self(),
            CanAsThough asThough => asThough.Objects ?? new Self(),
            CanBlock canBlock => canBlock.Objects ?? new Self(),
            CantBeActivated cantActivate => AbilityHolders(cantActivate.Abilities),
            Animate animate => animate.Objects ?? new Self(),
            // Prevention is read by the damage rules (Game.Prevent), not applied to an object's characteristics.
            PreventDamage => new Self(),
            // So are permissions to play from elsewhere (Game.StaticPermitted).
            MayPlayFrom => new Self(),
            // And control (Game.UpdateControl).
            GainControl gain => gain.Objects ?? new Self(),
            // And land plays (Game.LandPlays).
            ExtraLandPlays => new Self(),
            // And untapping (Game.DoesntUntap).
            DoesntUntap doesnt => doesnt.Objects ?? new Self(),
            // And blocking requirements (Game.DeclareBlockers).
            MustBlock must => must.Blockers ?? new Self(),
            _ => null,
        };

    /// <summary>
    /// What "its activated abilities", "activated abilities of artifacts" belong to: the selector the abilities are of,
    /// with a reference ("its", on an aura) read as the enchanted permanent.
    /// </summary>
    public static Selector AbilityHolders(Selector abilities) =>
        abilities switch
        {
            ObjectSelector { Filter.With: { } with } when with.OfType<Related>().FirstOrDefault(x => x.Relation == Relation.AbilityOf) is { } of =>
                of.To is Reference ? new ObjectSelector(Quantifier.Attached, null) : of.To,
            ObjectSelector { Filter.Kinds: { } kinds } selector when kinds.Any(x => x.Value == nameof(ObjectKind.Ability)) =>
                selector with { Filter = selector.Filter with { Kinds = kinds.Where(x => x.Value != nameof(ObjectKind.Ability)).ToList() is { Count: > 0 } rest ? rest : null, Qualifiers = selector.Filter.Qualifiers?.Where(q => q.Vocabulary != "AbilityCategory").ToList() } },
            _ => abilities ?? new Self(),
        };

    /// <summary>Whether a static effect from <paramref name="source"/> applies to <paramref name="obj"/>: its selector, matched without statics (so anthems don't recurse).</summary>
    /// <summary>Whether a static's selector, read from <paramref name="source"/>, covers <paramref name="obj"/>.</summary>
    public static bool Covers(GameState state, Selector objects, GameObject source, GameObject obj) => Applies(state, objects, source, obj);

    static bool Applies(GameState state, Selector objects, GameObject source, GameObject obj)
    {
        if (obj.Zone != Zone.Battlefield)
            return false;

        switch (objects)
        {
            case Self:
                return obj.Id == source.Id;
            case ObjectSelector { Quantifier: Quantifier.Attached }:
                return source.AttachedTo == obj.Id;
            case ObjectSelector { Quantifier: Quantifier.All or Quantifier.Each or Quantifier.Any } selector:
                return Filters.Matches(state, selector.Filter, obj, source.Controller, source.Id, selector.Other ? source.Id : null, withStatics: false);
            case Union union:
                return union.Members.Any(member => Applies(state, member, source, obj));
            default:
                return false;
        }
    }
}

/// <summary>A change to an object's card types, subtypes and colors: added to what it has, or instead of it.</summary>
public sealed record TypeChange(ImmutableList<string> Types = null, ImmutableList<string> Subtypes = null, ImmutableList<Color> Colors = null, bool InAddition = false, ImmutableList<string> Removes = null)
{
    /// <summary>The change "is the chosen type", "are mountains", "becomes an artifact" describes; null when an aspect isn't one the engine can apply.</summary>
    public static TypeChange OfAspects(IEnumerable<Term> aspects, bool inAddition, GameObject source)
    {
        var types = ImmutableList.CreateBuilder<string>();
        var subtypes = ImmutableList.CreateBuilder<string>();
        ImmutableList<Color>.Builder colors = null;
        foreach (var aspect in aspects)
            switch (aspect.Vocabulary)
            {
                case "CardType":
                    types.Add(aspect.Value);
                    break;
                case "CreatureType" or "LandType" or "ArtifactType" or "EnchantmentType" or "PermanentSubtype":
                    subtypes.Add(aspect.Value);
                    break;
                case "ManaColor":
                    colors ??= ImmutableList.CreateBuilder<Color>();
                    if (MTGEngine.Core.Colors.OfName(aspect.Value) is { } color)
                        colors.Add(color);
                    break;
                case "TypeDesignation" when aspect.Value == "ChosenType" && (source?.Chosen("CreatureType") ?? source?.Chosen("LandType")) is { } chosenType:
                    subtypes.Add(chosenType);
                    break;
                case "TypeDesignation" when aspect.Value == "ChosenColor" && MTGEngine.Core.Colors.OfName(source?.Chosen("Color")) is { } chosenColor:
                    colors ??= ImmutableList.CreateBuilder<Color>();
                    colors.Add(chosenColor);
                    break;
                case "TypeDesignation" when aspect.Value is "ChosenType" or "ChosenColor":
                    return new TypeChange(InAddition: true);
                default:
                    return null;
            }
        // Adding a creature type to a creature is always "in addition"; a type word alone ("is an artifact") adds to what it is.
        return new TypeChange(types.ToImmutable(), subtypes.ToImmutable(), colors?.ToImmutable(), inAddition || types.Count > 0 && subtypes.Count == 0 || subtypes.Count > 0 && subtypes.All(x => !Characteristics.IsLandType(x)) && types.Count == 0);
    }

    /// <summary>The type change of "becomes a 3/3 red elemental creature [that's still a land]".</summary>
    public static TypeChange OfAnimate(Animate animate)
    {
        if (animate.Types is not { Count: > 0 } && animate.Subtypes is not { Count: > 0 } && animate.Colors is not { Count: > 0 })
            return null;
        var keep = animate.InAddition || animate.Still is not null;
        var colors = animate.Colors?.Select(MTGEngine.Core.Colors.OfName).Where(x => x is not null).Select(x => x.Value).ToImmutableList();
        var types = (animate.Types ?? []).Select(CardDefinition.Normalize).ToImmutableList();
        if (animate.Still is { } still)
            types = types.Add(CardDefinition.Normalize(still));
        return new TypeChange(types, animate.Subtypes?.ToImmutableList(), colors is { Count: > 0 } ? colors : null, keep);
    }
}
