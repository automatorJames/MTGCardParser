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
    ImmutableList<Ability> Abilities)
{
    public bool HasKeyword(string keyword) => Keywords.Contains(CardDefinition.Normalize(keyword));

    public bool IsCreature => Types.Contains("Creature");

    /// <summary>Whether a combat action is denied: "Attack", "Block", "BeBlocked".</summary>
    public bool Cant(string action) => Restrictions.Contains(action);

    [ThreadStatic] static int _judgingCondition;

    static readonly ImmutableHashSet<string> _landTypes =
        ["Plains", "Island", "Swamp", "Mountain", "Forest", "Desert", "Gate", "Cave", "Lair", "Locus", "Mine", "Power-Plant", "Tower", "Sphere", "Town", "Urza's"];

    public static bool IsLandType(string subtype) => _landTypes.Contains(subtype);

    public static Characteristics Of(GameState state, GameObject obj, bool withStatics = true)
    {
        // A static's condition is judged on characteristics without statics, so conditions can't recurse into themselves.
        withStatics &= _judgingCondition == 0;
        var card = obj.Card;
        var power = card.Power ?? 0;
        var toughness = card.Toughness ?? 0;
        var keywords = card.Keywords.ToBuilder();
        var restrictions = ImmutableHashSet.CreateBuilder<string>();
        var abilities = card.Abilities.ToBuilder();
        var types = card.Types;
        var subtypes = card.Subtypes;
        var colors = card.Colors;
        var lost = false;

        var temporary = state.Temporary.Where(x => x.Target == obj.Id).ToList();
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

        foreach (var (effect, _, _) in statics)
        {
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
            foreach (var ability in effect.Abilities ?? [])
            {
                abilities.Add(ability);
                if (ability is KeywordLine line)
                    foreach (var keyword in line.Keywords)
                        keywords.Add(CardDefinition.Normalize(keyword.Keyword));
            }
        }

        foreach (var (effect, _, _) in statics)
            switch (effect)
            {
                case Pump pump:
                {
                    var factor = pump.Remove ? -1 : 1;
                    power += factor * ((pump.Power as Fixed)?.Value ?? 0);
                    toughness += factor * ((pump.Toughness as Fixed)?.Value ?? 0);
                    foreach (var keyword in pump.Keywords ?? [])
                        if (pump.Remove)
                            keywords.Remove(CardDefinition.Normalize(keyword.Keyword));
                        else
                            keywords.Add(CardDefinition.Normalize(keyword.Keyword));
                    break;
                }
                case Animate animate:
                    foreach (var keyword in animate.Keywords ?? [])
                        keywords.Add(CardDefinition.Normalize(keyword.Keyword));
                    break;
                case CombatRestriction restriction:
                    foreach (var cant in restriction.Cant)
                        restrictions.Add(cant);
                    break;
                case GrantAbility grant:
                    abilities.Add(grant.Ability);
                    if (grant.Ability is KeywordLine line)
                        foreach (var keyword in line.Keywords)
                            keywords.Add(CardDefinition.Normalize(keyword.Keyword));
                    foreach (var keyword in grant.Alongside ?? [])
                        keywords.Add(CardDefinition.Normalize(keyword.Keyword));
                    break;
                case LoseAllAbilities:
                    lost = true;
                    keywords.Clear();
                    abilities.Clear();
                    break;
            }

        return new Characteristics(power, toughness, keywords.ToImmutable(), types, subtypes, colors, lost, restrictions.ToImmutable(), abilities.ToImmutable());
    }

    /// <summary>The abilities an object has now: printed, granted for a while, and granted by statics on the battlefield.</summary>
    public static ImmutableList<Ability> AbilitiesOf(GameState state, GameObject obj) => Of(state, obj).Abilities;

    /// <summary>The type, subtype and color change a static effect makes, read for <paramref name="source"/> (whose choices "the chosen type" means); null for none.</summary>
    public static TypeChange TypeChangeOf(Effect effect, GameObject source) =>
        effect switch
        {
            BecomesAspect aspect => TypeChange.OfAspects(aspect.Aspects, aspect.InAddition, source),
            Animate animate => TypeChange.OfAnimate(animate),
            _ => null,
        };

    /// <summary>The continuous effects of static abilities on the battlefield whose condition (if any) holds now: each effect with what it applies to and its source.</summary>
    public static IEnumerable<(Effect Effect, Selector Objects, GameObject Source)> StaticEffects(GameState state)
    {
        foreach (var source in state.Battlefield)
        {
            if (state.Temporary.Any(x => x.Target == source.Id && x.LosesAbilities))
                continue;
            // Printed and temporarily granted statics; statics granted by other statics would recurse, so only those.
            var abilities = source.Card.Abilities.Concat(state.Temporary.Where(x => x.Target == source.Id).SelectMany(x => x.Abilities ?? []));
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
                foreach (var effect in Continuous(ability.Effect))
                    if (ObjectsOf(effect) is { } objects)
                        yield return (effect, objects, source);
            }
        }
    }

    static IEnumerable<Effect> Continuous(Effect effect) =>
        effect switch
        {
            Pump { Duration: null } => [effect],
            CombatRestriction { Duration: null, If: null } => [effect],
            GrantAbility => [effect],
            LoseAllAbilities => [effect],
            BecomesAspect { Duration: null, Negated: false } => [effect],
            Animate { Duration: null, Transforms: false } => [effect],
            PreventDamage { Duration: null } => [effect],
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
            Animate animate => animate.Objects ?? new Self(),
            // Prevention is read by the damage rules (Game.Prevent), not applied to an object's characteristics.
            PreventDamage => new Self(),
            _ => null,
        };

    /// <summary>Whether a static effect from <paramref name="source"/> applies to <paramref name="obj"/>: its selector, matched without statics (so anthems don't recurse).</summary>
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
public sealed record TypeChange(ImmutableList<string> Types = null, ImmutableList<string> Subtypes = null, ImmutableList<Color> Colors = null, bool InAddition = false)
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
