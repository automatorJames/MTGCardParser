namespace MTGEngine.Core;

/// <summary>Matching game objects against the IR's object filters: kinds, qualifier words, controller, zone and the predicates the engine understands.</summary>
public static class Filters
{
    /// <summary>Whether <paramref name="obj"/> satisfies <paramref name="filter"/>, read from the point of view of <paramref name="you"/> (the controller of the ability) and <paramref name="source"/> (the object it's on).</summary>
    public static bool Matches(GameState state, ObjectFilter filter, GameObject obj, PlayerId you, ObjectId? source, ObjectId? other = null, bool withStatics = true, EffectContext ctx = null)
    {
        if (other is { } excluded && obj.Id == excluded)
            return false;

        if (filter is null)
            return true;

        var traits = Characteristics.Of(state, obj, withStatics);

        if (filter.Kinds is { Count: > 0 } kinds && !kinds.Any(kind => KindMatches(kind, obj, traits)))
            return false;

        foreach (var qualifier in filter.Qualifiers ?? [])
            if (!QualifierMatches(qualifier, obj, traits, source))
                return false;

        if (filter.Controller is { } controlled)
        {
            var players = ctx is not null ? Resolver.Players(controlled.Player, ctx) : Resolver.Players(state, controlled.Player, you, source);
            var owner = controlled.Relation == "Own" ? obj.Owner : obj.Controller;
            var holds = players.Contains(owner);
            if (holds == controlled.Negated)
                return false;
        }

        if (filter.Zone is { } zone)
        {
            if (!ZoneMatches(state, zone, obj, you, ctx))
                return false;
        }
        else if (filter.Kinds is null || filter.Kinds.All(x => x.Vocabulary != nameof(ObjectKind) || x.Value is nameof(ObjectKind.Permanent) or nameof(ObjectKind.Any)))
        {
            // A filter with no zone describes permanents, unless it names cards or spells.
            if (obj.Zone != Zone.Battlefield && !(filter.Kinds?.Any(x => x.Value is nameof(ObjectKind.Spell) or nameof(ObjectKind.Card)) ?? false))
                return false;
        }

        foreach (var predicate in filter.With ?? [])
            if (!PredicateMatches(state, predicate, obj, traits, you, source, ctx))
                return false;

        return true;
    }

    public static bool KindMatches(Term kind, GameObject obj, Characteristics traits) =>
        kind.Vocabulary switch
        {
            "TokenWord" => obj.IsToken,
            "PlayerIdentity" => false,
            nameof(ObjectKind) => kind.Value switch
            {
                nameof(ObjectKind.Any) => true,
                nameof(ObjectKind.Card) => obj.Zone != Zone.Battlefield && obj.Zone != Zone.Stack && !obj.IsToken,
                nameof(ObjectKind.Permanent) => obj.Zone == Zone.Battlefield,
                nameof(ObjectKind.Spell) => obj.Zone == Zone.Stack,
                nameof(ObjectKind.Token) => obj.IsToken,
                nameof(ObjectKind.Source) => true,
                nameof(ObjectKind.Player) => false,
                _ => false,
            },
            "CardType" => traits.Types.Contains(kind.Value),
            "CreatureType" or "LandType" or "PermanentSubtype" or "PlaneswalkerType" or "ArtifactType" or "EnchantmentType" or "SpellSubtype" => traits.Subtypes.Contains(kind.Value),
            "Supertype" => obj.Card.Supertypes.Contains(kind.Value),
            "PermanentKind" => traits.Types.Contains(kind.Value) || traits.Subtypes.Contains(kind.Value),
            _ => false,
        };

    public static bool QualifierMatches(Qualifier qualifier, GameObject obj, Characteristics traits, ObjectId? source)
    {
        var holds = qualifier.Vocabulary switch
        {
            "CardType" => traits.Types.Contains(qualifier.Quality),
            "CreatureType" or "LandType" or "PermanentSubtype" or "PlaneswalkerType" or "ArtifactType" or "EnchantmentType" => traits.Subtypes.Contains(qualifier.Quality),
            "Supertype" => obj.Card.Supertypes.Contains(qualifier.Quality),
            "ManaColor" => Colors.OfName(qualifier.Quality) is { } color && traits.Colors.Contains(color),
            "ColorCount" => qualifier.Quality switch
            {
                "Colorless" => traits.Colors.Count == 0,
                "Monocolored" => traits.Colors.Count == 1,
                "Multicolored" => traits.Colors.Count > 1,
                "OneOrMoreColors" => traits.Colors.Count > 0,
                _ => false,
            },
            "TapState" => qualifier.Quality == "Tapped" ? obj.Tapped : !obj.Tapped,
            "CombatState" => qualifier.Quality switch
            {
                "Attacking" => obj.Attacking,
                "Blocking" => obj.Blocking is not null,
                "Blocked" => false,
                "Unblocked" => obj.Attacking,
                "AttackingAlone" => obj.Attacking,
                _ => false,
            },
            nameof(ObjectKind) => qualifier.Quality switch
            {
                nameof(ObjectKind.Token) => obj.IsToken,
                nameof(ObjectKind.Permanent) => obj.Zone == Zone.Battlefield,
                _ => false,
            },
            "Distinction" => qualifier.Quality == nameof(Distinction.Other) ? obj.Id != source : false,
            "Keyword" => traits.HasKeyword(qualifier.Quality),
            "Position" => true,
            "Provenance" => true,
            "AttachedBy" => true,
            "Kind" => qualifier.Quality == "Token" ? obj.IsToken : qualifier.Quality == "Nontoken" ? !obj.IsToken : false,
            "TokenWord" => qualifier.Quality == "Token" ? obj.IsToken : false,
            "CounterKind" => true,
            _ => false,
        };

        return holds != qualifier.Negated;
    }

    static bool ZoneMatches(GameState state, ZoneRef zone, GameObject obj, PlayerId you, EffectContext ctx)
    {
        if (!Enum.TryParse<Zone>(zone.Zone, out var wanted))
            return false;
        if (obj.Zone != wanted)
            return false;
        if (zone.Owner is null)
            return true;
        var owners = ctx is not null ? Resolver.Players(zone.Owner, ctx) : Resolver.Players(state, zone.Owner, you, null);
        return owners.Contains(wanted == Zone.Battlefield ? obj.Controller : obj.Owner);
    }

    static bool PredicateMatches(GameState state, Predicate predicate, GameObject obj, Characteristics traits, PlayerId you, ObjectId? source, EffectContext ctx)
    {
        switch (predicate)
        {
            case HasKeywords has:
                return has.Keywords.All(traits.HasKeyword) != has.Negated;
            case HasStats stats:
                return (stats.Power is not Fixed p || traits.Power == p.Value) && (stats.Toughness is not Fixed t || traits.Toughness == t.Value);
            case CharacteristicBound bound when ctx is not null:
            {
                var threshold = Quantities.Evaluate(bound.Value, ctx);
                if (threshold is null)
                    return false;
                return bound.Characteristics.All(characteristic => Compare(CharacteristicValue(characteristic, obj, traits), bound.Comparison, threshold.Value));
            }
            case CharacteristicBound bound:
                return bound.Value is Fixed fixedValue && bound.Characteristics.All(characteristic => Compare(CharacteristicValue(characteristic, obj, traits), bound.Comparison, fixedValue.Value));
            case Related { Relation: Relation.OtherThan } related:
                return ctx is null ? obj.Id != source : !Resolver.Objects(related.To, ctx).Contains(obj.Id);
            case Related { Relation: Relation.AttachedTo } related:
                return obj.AttachedTo is { } host && (ctx is null ? host == source : Resolver.Objects(related.To, ctx).Contains(host));
            case Related { Relation: Relation.Among } related when ctx is not null:
                return Resolver.Objects(related.To, ctx).Contains(obj.Id);
            case Related { Relation: Relation.Is } related when ctx is not null:
                return Resolver.Objects(related.To, ctx).Contains(obj.Id);
            case QualifierChoice choice:
                return choice.Conjunction == Connective.And
                    ? choice.Options.All(x => QualifierMatches(x, obj, traits, source))
                    : choice.Options.Any(x => QualifierMatches(x, obj, traits, source));
            case QualifiedKind qualified:
                return KindMatches(qualified.Kind, obj, traits) && qualified.Qualifiers.All(x => QualifierMatches(x, obj, traits, source));
            case CastBy { Relation: "Control" } cast when ctx is not null:
                return Resolver.Players(cast.Player, ctx).Contains(obj.Controller);
            case WithCounters counters:
                return counters.Counters.Kind is { } kind
                    ? (counters.Counters.Count is Fixed count ? obj.CountersOf(kind) >= count.Value : obj.CountersOf(kind) > 0)
                    : obj.CounterMap.Count > 0;
            case ChosenBy:
                return true;
            case Satisfying satisfying when ctx is not null:
                return Conditions.Evaluate(satisfying.Condition, ctx with { Subject = TargetRef.Of(obj.Id) }) == true;
            default:
                return false;
        }
    }

    public static int CharacteristicValue(string characteristic, GameObject obj, Characteristics traits) =>
        characteristic switch
        {
            "Power" => traits.Power,
            "Toughness" => traits.Toughness,
            "ManaValue" => obj.Card.ManaCost.ManaValue,
            _ => 0,
        };

    public static bool Compare(int left, Comparison comparison, int right) =>
        comparison switch
        {
            Comparison.Equal => left == right,
            Comparison.Less => left < right,
            Comparison.LessOrEqual => left <= right,
            Comparison.Greater => left > right,
            Comparison.GreaterOrEqual => left >= right,
            _ => false,
        };
}
