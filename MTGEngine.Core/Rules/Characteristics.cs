namespace MTGEngine.Core;

/// <summary>
/// An object's characteristics as they are now: printed values, then counters, temporary effects, and the static
/// abilities of permanents on the battlefield. A light version of the layer system: enough for anthems, pumps,
/// +1/+1 counters, keyword grants, granted abilities, combat restrictions and losing all abilities.
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
        var lost = false;

        var temporary = state.Temporary.Where(x => x.Target == obj.Id).ToList();
        var statics = withStatics ? StaticEffects(state).Where(x => Applies(state, x.Objects, x.Source, obj)).ToList() : [];

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
            if (effect is Pump { BaseStats: { } stats })
            {
                if (stats.Power is Fixed p) power = p.Value;
                if (stats.Toughness is Fixed t) toughness = t.Value;
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

        return new Characteristics(power, toughness, keywords.ToImmutable(), card.Types, card.Subtypes, card.Colors, lost, restrictions.ToImmutable(), abilities.ToImmutable());
    }

    /// <summary>The abilities an object has now: printed, granted for a while, and granted by statics on the battlefield.</summary>
    public static ImmutableList<Ability> AbilitiesOf(GameState state, GameObject obj) => Of(state, obj).Abilities;

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
