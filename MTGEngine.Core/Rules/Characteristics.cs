namespace MTGEngine.Core;

/// <summary>
/// An object's characteristics as they are now: printed values, then counters, temporary effects, and the static
/// abilities of permanents on the battlefield. A light version of the layer system: enough for anthems, pumps,
/// +1/+1 counters, keyword grants and losing all abilities.
/// </summary>
public sealed record Characteristics(int Power, int Toughness, ImmutableHashSet<string> Keywords, ImmutableHashSet<string> Types, ImmutableHashSet<string> Subtypes, ImmutableHashSet<Color> Colors, bool LostAbilities)
{
    public bool HasKeyword(string keyword) => Keywords.Contains(CardDefinition.Normalize(keyword));

    public bool IsCreature => Types.Contains("Creature");

    public static Characteristics Of(GameState state, GameObject obj, bool withStatics = true)
    {
        var card = obj.Card;
        var power = card.Power ?? 0;
        var toughness = card.Toughness ?? 0;
        var keywords = card.Keywords.ToBuilder();
        var types = card.Types;
        var subtypes = card.Subtypes;
        var colors = card.Colors;
        var lost = false;

        foreach (var effect in state.Temporary.Where(x => x.Target == obj.Id))
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
            }
        }

        if (withStatics)
            foreach (var (pump, source) in StaticPumps(state))
                if (Applies(state, pump, source, obj))
                {
                    if (pump.BaseStats is { } stats)
                    {
                        if (stats.Power is Fixed p) power = p.Value;
                        if (stats.Toughness is Fixed t) toughness = t.Value;
                    }
                }

        power += obj.CountersOf("+1/+1") - obj.CountersOf("-1/-1");
        toughness += obj.CountersOf("+1/+1") - obj.CountersOf("-1/-1");

        foreach (var effect in state.Temporary.Where(x => x.Target == obj.Id))
        {
            power += effect.Power;
            toughness += effect.Toughness;
            foreach (var keyword in effect.Keywords)
                keywords.Add(CardDefinition.Normalize(keyword));
        }

        if (withStatics)
            foreach (var (pump, source) in StaticPumps(state))
                if (Applies(state, pump, source, obj))
                {
                    var factor = pump.Remove ? -1 : 1;
                    power += factor * ((pump.Power as Fixed)?.Value ?? 0);
                    toughness += factor * ((pump.Toughness as Fixed)?.Value ?? 0);
                    foreach (var keyword in pump.Keywords ?? [])
                        if (pump.Remove)
                            keywords.Remove(CardDefinition.Normalize(keyword.Keyword));
                        else
                            keywords.Add(CardDefinition.Normalize(keyword.Keyword));
                }

        return new Characteristics(power, toughness, keywords.ToImmutable(), types, subtypes, colors, lost);
    }

    /// <summary>The continuous pumps on the battlefield: each static ability whose effect is a pump with no end, with its source.</summary>
    static IEnumerable<(Pump Pump, GameObject Source)> StaticPumps(GameState state)
    {
        foreach (var source in state.Battlefield)
        {
            if (state.Temporary.Any(x => x.Target == source.Id && x.LosesAbilities))
                continue;
            foreach (var ability in source.Card.Abilities.OfType<StaticAbility>())
                foreach (var pump in Pumps(ability.Effect))
                    if (ability.While is null)
                        yield return (pump, source);
        }
    }

    static IEnumerable<Pump> Pumps(Effect effect) =>
        effect switch
        {
            Pump { Duration: null } pump => [pump],
            Sequence sequence => sequence.Steps.SelectMany(Pumps),
            _ => [],
        };

    /// <summary>Whether a static pump from <paramref name="source"/> applies to <paramref name="obj"/>: the pump's selector, matched without statics (so anthems don't recurse).</summary>
    static bool Applies(GameState state, Pump pump, GameObject source, GameObject obj)
    {
        if (obj.Zone != Zone.Battlefield)
            return false;

        switch (pump.Objects)
        {
            case Self:
                return obj.Id == source.Id;
            case ObjectSelector { Quantifier: Quantifier.Attached }:
                return source.AttachedTo == obj.Id;
            case ObjectSelector { Quantifier: Quantifier.All or Quantifier.Each or Quantifier.Any } selector:
                return Filters.Matches(state, selector.Filter, obj, source.Controller, source.Id, selector.Other ? source.Id : null, withStatics: false);
            default:
                return false;
        }
    }
}
