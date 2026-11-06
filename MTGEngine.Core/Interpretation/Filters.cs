namespace MTGEngine.Core;

/// <summary>Matching game objects against the IR's object filters: kinds, qualifier words, controller, zone and the predicates the engine understands.</summary>
public static class Filters
{
    static readonly ImmutableHashSet<string> _permanentTypes = ["Artifact", "Battle", "Creature", "Enchantment", "Land", "Planeswalker"];

    /// <summary>Whether <paramref name="obj"/> satisfies <paramref name="filter"/>, read from the point of view of <paramref name="you"/> (the controller of the ability) and <paramref name="source"/> (the object it's on).</summary>
    public static bool Matches(GameState state, ObjectFilter filter, GameObject obj, PlayerId you, ObjectId? source, ObjectId? other = null, bool withStatics = true, EffectContext ctx = null)
    {
        if (other is { } excluded && obj.Id == excluded)
            return false;

        if (filter is null)
            return true;

        var traits = Characteristics.Of(state, obj, withStatics);

        if (filter.Kinds is { Count: > 0 } kinds && !kinds.Any(kind => KindMatches(kind, obj, traits, state)))
            return false;

        // "The second spell you cast each turn": count what its controller cast before it this turn that fits the rest of the filter.
        if (filter.Qualifiers?.FirstOrDefault(q => q.Vocabulary == "SpellOrdinal") is { } ordinal)
        {
            var wanted = ordinal.Quality switch { "First" => 1, "Second" => 2, "Third" => 3, _ => 0 };
            var rest = filter with { Qualifiers = filter.Qualifiers.Where(q => q != ordinal).ToList() };
            if (!Matches(state, rest, obj, you, source, other, withStatics, ctx))
                return false;
            if (wanted == 0)
                return true;
            var before = state.ThisTurnEventList.OfType<SpellCast>().TakeWhile(x => x.Item.Spell != obj.Id)
                .Count(x => x.Item.Controller == obj.Controller && x.Item.Spell is { } earlier && state.Object(earlier) is { } spell && Matches(state, rest with { Zone = null }, spell with { Zone = Zone.Stack }, you, source, null, withStatics, ctx));
            return before + 1 == wanted != ordinal.Negated;
        }

        // "Instant and sorcery cards" names two types no card has both of: either will do.
        var qualifiers = filter.Qualifiers ?? [];
        var eitherSpellType = qualifiers.Count(q => q.Vocabulary == "CardType" && !q.Negated && q.Quality is "Instant" or "Sorcery") == 2;
        foreach (var qualifier in qualifiers)
            if (eitherSpellType && qualifier.Vocabulary == "CardType" && !qualifier.Negated && qualifier.Quality is "Instant" or "Sorcery")
            {
                if (!traits.Types.Contains("Instant") && !traits.Types.Contains("Sorcery"))
                    return false;
            }
            else if (!QualifierMatches(qualifier, obj, traits, source, state))
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
            if (obj.Zone != Zone.Battlefield && !(filter.Kinds?.Any(x => x.Value is nameof(ObjectKind.Spell) or nameof(ObjectKind.Card) or nameof(ObjectKind.Source)) ?? false))
                return false;
        }

        foreach (var predicate in filter.With ?? [])
            if (predicate is HasExtreme extreme)
            {
                if (!IsExtreme(state, filter, extreme, obj, traits, you, source, ctx))
                    return false;
            }
            else if (!PredicateMatches(state, predicate, obj, traits, you, source, ctx))
                return false;

        return true;
    }

    /// <summary>"The creature with the greatest power among creatures you control", "each creature with the least toughness": the most (or least) among the filter's other matches, ties included.</summary>
    static bool IsExtreme(GameState state, ObjectFilter filter, HasExtreme extreme, GameObject obj, Characteristics traits, PlayerId you, ObjectId? source, EffectContext ctx)
    {
        if (_comparing > 2)
            return false;
        _comparing++;
        try
        {
            var rest = filter with { With = filter.With.Where(x => x is not HasExtreme).ToList() };
            var among = extreme.Among is ObjectSelector { Filter: var amongFilter } ? amongFilter : rest;
            var values = state.Battlefield.Where(x => Matches(state, among, x, you, source, ctx: ctx)).Select(x => CharacteristicValue(extreme.Characteristic, x, Characteristics.Of(state, x))).ToList();
            var mine = CharacteristicValue(extreme.Characteristic, obj, traits);
            if (values.Count == 0)
                return true;
            return extreme.Which is "Least" or "Lowest" or "Smallest" ? mine <= values.Min() : mine >= values.Max();
        }
        finally { _comparing--; }
    }

    public static bool KindMatches(Term kind, GameObject obj, Characteristics traits, GameState state = null) =>
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
            "CardType" when kind.Value == nameof(ObjectKind.Permanent) => obj.Zone == Zone.Battlefield,
            "CardType" => traits.Types.Contains(kind.Value),
            "CreatureType" => traits.Subtypes.Contains(kind.Value) || traits.HasKeyword("Changeling"),
            "LandType" or "PermanentSubtype" or "PlaneswalkerType" or "ArtifactType" or "EnchantmentType" or "SpellSubtype" => traits.Subtypes.Contains(kind.Value),
            "Supertype" => obj.Card.Supertypes.Contains(kind.Value),
            "PermanentKind" => traits.Types.Contains(kind.Value) || traits.Subtypes.Contains(kind.Value),
            "PermanentDesignation" => Designated(state, obj, kind.Value),
            "ManaColor" => Colors.OfName(kind.Value) is { } color ? traits.Colors.Contains(color) : kind.Value == "Colorless" && traits.Colors.Count == 0,
            "TapState" => kind.Value == "Tapped" ? obj.Tapped : !obj.Tapped,
            // "The copy", "the copies", "that spell": a pool the resolver supplies (what the copying step made).
            "RetargetedObject" or "CopiedSpell" => true,
            "SourceNoun" => kind.Value != nameof(ObjectKind.Spell) || obj.Zone == Zone.Stack,
            "CombatState" => kind.Value switch { "Attacking" => obj.Attacking, "Blocking" => obj.Blocking is not null, _ => false },
            _ => false,
        };

    /// <summary>The designations the engine tracks: equipped, enchanted, face down, monstrous, modified.</summary>
    public static readonly ImmutableHashSet<string> Designations = ["Equipped", "Enchanted", "FaceDown", "Monstrous", "Modified", "Saddled"];

    public static bool Designated(GameState state, GameObject obj, string designation)
    {
        bool Attached(Func<GameObject, bool> kind) => state?.Battlefield.Any(x => x.AttachedTo == obj.Id && kind(x)) == true;
        return designation switch
        {
            "Equipped" => Attached(x => x.Card.IsEquipment),
            "Enchanted" => Attached(x => x.Card.IsAura),
            "FaceDown" => obj.FaceDown,
            "Saddled" => state is not null && obj.Chosen("SaddledTurn") == state.Turn.ToString(),
            "Monstrous" => obj.CountersOf("monstrous") > 0,
            "Modified" => obj.CounterMap.Keys.Any(k => k is not ("regeneration-shield" or "deathtouch-damage" or "monstrous")) || Attached(x => x.Card.IsEquipment || x.Card.IsAura && x.Controller == obj.Controller),
            _ => false,
        };
    }

    public static bool QualifierMatches(Qualifier qualifier, GameObject obj, Characteristics traits, ObjectId? source, GameState state = null)
    {
        var holds = qualifier.Vocabulary switch
        {
            // "Permanent card": an artifact, battle, creature, enchantment, land or planeswalker card.
            "CardType" when qualifier.Quality == nameof(ObjectKind.Permanent) => obj.Zone == Zone.Battlefield || traits.Types.Overlaps(_permanentTypes),
            "CardType" => traits.Types.Contains(qualifier.Quality),
            "CreatureType" => traits.Subtypes.Contains(qualifier.Quality) || traits.HasKeyword("Changeling"),
            "LandType" or "PermanentSubtype" or "PlaneswalkerType" or "ArtifactType" or "EnchantmentType" => traits.Subtypes.Contains(qualifier.Quality),
            "Supertype" => obj.Card.Supertypes.Contains(qualifier.Quality),
            "ManaColor" => Colors.OfName(qualifier.Quality) is { } color && traits.Colors.Contains(color),
            "ColorCount" or "Colorfulness" => qualifier.Quality switch
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
                "Blocked" => state is not null && History.WasBlocked(state, obj.Id),
                "Unblocked" => obj.Attacking && (state is null || !History.WasBlocked(state, obj.Id)),
                "AttackingAlone" => obj.Attacking,
                _ => false,
            },
            nameof(ObjectKind) => qualifier.Quality switch
            {
                nameof(ObjectKind.Token) => obj.IsToken,
                nameof(ObjectKind.Permanent) => obj.Zone == Zone.Battlefield,
                _ => false,
            },
            "Distinction" => qualifier.Quality switch
            {
                nameof(Distinction.Other) => obj.Id != source,
                // "Cards named {this}".
                nameof(Distinction.NamedThis) => source is { } named && state?.Object(named) is { } self && obj.Card.Name == self.Card.Name,
                // "The creature with the greatest toughness or tied for it": ties count already (see IsExtreme).
                nameof(Distinction.OrTied) => true,
                nameof(Distinction.NotOnTheBattlefield) => obj.Zone != Zone.Battlefield,
                _ => false,
            },
            "Keyword" => traits.HasKeyword(qualifier.Quality),
            // "Historic" (artifacts, legendaries and sagas), "outlaw" (assassins, mercenaries, pirates, rogues and warlocks).
            "CardGroup" => qualifier.Quality switch
            {
                "Historic" => traits.Types.Contains("Artifact") || obj.Card.Supertypes.Contains("Legendary") || traits.Subtypes.Contains("Saga"),
                "Outlaw" => traits.Subtypes.Overlaps(_outlaws),
                _ => false,
            },
            // "Up to one other target creature"; "one or more target creatures" (how many is the selector's).
            "CountSuffix" => qualifier.Quality switch
            {
                "Other" => obj.Id != source,
                "OrMore" => true,
                _ => false,
            },
            // "A card at random": how it's picked (Resolver), not which cards qualify.
            "Manner" => true,
            // "Abilities", "an ability": the kind of thing, read with what it's an ability of.
            "AbilitiesWord" => true,
            "Position" => true,
            "Provenance" => true,
            "AttachedBy" => true,
            "Kind" => qualifier.Quality == "Token" ? obj.IsToken : qualifier.Quality == "Nontoken" ? !obj.IsToken : false,
            "TokenWord" => qualifier.Quality == "Token" ? obj.IsToken : false,
            "CounterKind" => true,
            "PermanentDesignation" => Designated(state, obj, qualifier.Quality),
            // Leftovers and "… this way" name a pool the resolver supplies; within it they hold.
            // "Each creature dealt damage this turn".
            "DamageHistory" when qualifier.Quality == "DealtDamageThisTurn" => state is null || state.ThisTurnEventList.OfType<DamageDealt>().Any(x => x.Target.IsObject && x.Target.Object == obj.Id),
            "LeftoverCards" or "AffectedThisWay" or "DamageHistory" or "ThisWayAction" => true,
            // "Each turn" in "your second spell each turn" is the period the ordinal counts over; "activated" in "activated abilities of …" is read where abilities are.
            "SpellCountPeriod" or "AbilityCategory" => true,
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

    [ThreadStatic] static int _comparing;

    /// <summary>"Creatures with different powers", "cards with different names": one of each value, when the filter asks for it.</summary>
    public static IEnumerable<T> DistinctBy<T>(IEnumerable<T> items, ObjectFilter filter, GameState state, Func<T, GameObject> objectOf)
    {
        if (filter?.With?.OfType<Distinct>().FirstOrDefault() is not { } distinct)
            return items;
        return items.Where(x => objectOf(x) is not null).DistinctBy(x =>
        {
            var obj = objectOf(x);
            return distinct.Characteristic == "Name" ? obj.Card.Name : CharacteristicValue(distinct.Characteristic, obj, Characteristics.Of(state, obj)).ToString();
        });
    }

    static readonly ImmutableHashSet<string> _outlaws = ["Assassin", "Mercenary", "Pirate", "Rogue", "Warlock"];

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
            // "With different names / powers": a property of the set, applied where the set is resolved (DistinctBy).
            case Distinct:
                return true;
            // "With power less than this creature's power", "with toughness greater than its power".
            case CharacteristicCompared compared:
            {
                var other = compared.Other switch
                {
                    Self => source is { } own ? state.Object(own) : null,
                    Reference { Text: "its" } => obj,
                    _ when ctx is not null => Resolver.Objects(compared.Other, ctx).Select(state.Object).FirstOrDefault(x => x is not null),
                    _ => null,
                };
                if (other is null || _comparing > 2)
                    return false;
                _comparing++;
                try
                {
                    var otherTraits = other.Id == obj.Id ? traits : Characteristics.Of(state, other);
                    return Compare(CharacteristicValue(compared.Characteristic, obj, traits), compared.Comparison, CharacteristicValue(compared.OtherCharacteristic, other, otherTraits));
                }
                finally { _comparing--; }
            }
            case CharacteristicBound bound:
                return bound.Value is Fixed fixedValue && bound.Characteristics.All(characteristic => Compare(CharacteristicValue(characteristic, obj, traits), bound.Comparison, fixedValue.Value));
            case Related { Relation: Relation.OtherThan } related:
                return ctx is null ? obj.Id != source : !Resolver.Objects(related.To, ctx).Contains(obj.Id);
            case Related { Relation: Relation.AttachedTo } related:
                return obj.AttachedTo is { } host && (ctx is null ? host == source : Resolver.Objects(related.To, ctx).Contains(host));
            case Related { Relation: Relation.ExiledWith } exiledWith:
            {
                var holders = exiledWith.To is Self ? (source is { } own ? [own] : []) : ctx is not null ? Resolver.Objects(exiledWith.To, ctx) : [];
                return obj.Zone == Zone.Exile && state.ExileLinkList.Any(x => holders.Contains(x.Source) && x.Card == obj.Id && x.CardZoneChanges == obj.ZoneChangeCount);
            }
            // "A creature dealt damage by this creature this turn".
            case Related { Relation: Relation.DealtDamageBy } dealtBy:
            {
                var dealers = dealtBy.To is Self ? (source is { } own ? [own] : []) : ctx is not null ? Resolver.Objects(dealtBy.To, ctx) : [];
                return state.ThisTurnEventList.OfType<DamageDealt>().Any(x => x.Target.IsObject && x.Target.Object == obj.Id && x.Source is { } dealer && dealers.Contains(dealer));
            }
            case Related { Relation: Relation.Among } related when ctx is not null:
                return Resolver.Objects(related.To, ctx).Contains(obj.Id);
            case Related { Relation: Relation.Is } related when ctx is not null:
                return Resolver.Objects(related.To, ctx).Contains(obj.Id);
            case QualifierChoice choice:
                return choice.Conjunction == Connective.And
                    ? choice.Options.All(x => QualifierMatches(x, obj, traits, source, state))
                    : choice.Options.Any(x => QualifierMatches(x, obj, traits, source, state));
            case QualifiedKind qualified:
                return KindMatches(qualified.Kind, obj, traits, state) && qualified.Qualifiers.All(x => QualifierMatches(x, obj, traits, source, state));
            case CastBy { Relation: "Control" } cast when ctx is not null:
                return Resolver.Players(cast.Player, ctx).Contains(obj.Controller);
            case CastBy { Relation: "Cast", ThisTurn: false, From: null, During: null } cast:
                return obj.Zone == Zone.Stack && (ctx is not null ? Resolver.Players(cast.Player, ctx) : Resolver.Players(state, cast.Player, you, source)).Contains(obj.Controller);
            // "Creatures with no counters on them".
            case WithCounters { Counters.Count: Fixed { Value: 0 } } none:
                return none.Counters.Kind is { } noneOf ? obj.CountersOf(noneOf) == 0 : !obj.CounterMap.Any(x => x.Value > 0 && x.Key is not ("deathtouch-damage" or "regeneration-shield"));
            case WithCounters counters:
                return counters.Counters.Kind is { } kind
                    ? (counters.Counters.Count is Fixed count ? obj.CountersOf(kind) >= count.Value : obj.CountersOf(kind) > 0)
                    : obj.CounterMap.Count > 0;
            case ChosenBy:
                return true;
            case Targeting targeting:
            {
                var item = state.Stack.LastOrDefault(x => x.Spell == obj.Id) ?? state.Stack.LastOrDefault(x => x.Spell is null && x.Source == obj.Id);
                return item is not null && item.Targets.Any(target => targeting.Target switch
                {
                    Self => target.Object == source,
                    ObjectSelector selector => target.IsObject && state.Object(target.Object.Value) is { } targeted && Matches(state, selector.Filter, targeted, you, source, ctx: ctx),
                    PlayerSelector { Kind: PlayerKind.You } => target.Player == you,
                    PlayerSelector => target.IsPlayer,
                    _ => false,
                });
            }
            case Named named:
                return string.Equals(obj.Card.Name, named.Name, StringComparison.OrdinalIgnoreCase);
            case InCombatWith combat:
            {
                var others = combat.Other is Self ? (source is { } own ? [own] : []) : ctx is not null ? Resolver.Objects(combat.Other, ctx) : [];
                return others.Any(o => combat.Relation switch
                {
                    "Blocking" => obj.Blocking == o,
                    "BlockedBy" => state.Object(o)?.Blocking == obj.Id,
                    _ => obj.Blocking == o || state.Object(o)?.Blocking == obj.Id,
                });
            }
            case SharesCharacteristic shares:
            {
                var others = shares.With switch
                {
                    Self => source is { } own && state.Object(own) is { } self ? [self] : [],
                    _ when ctx is not null => Resolver.Objects(shares.With, ctx).Select(state.Object).Where(x => x is not null).ToList(),
                    _ => [],
                };
                return others.Any(other =>
                {
                    var theirs = Characteristics.Of(state, other, withStatics: false);
                    return shares.Characteristic switch
                    {
                        "Name" => other.Card.Name == obj.Card.Name,
                        "Color" => traits.Colors.Overlaps(theirs.Colors),
                        "CreatureType" => traits.Subtypes.Where(x => !Characteristics.IsLandType(x)).Any(theirs.Subtypes.Contains),
                        "CardType" => traits.Types.Overlaps(theirs.Types),
                        _ => false,
                    };
                });
            }
            case OfChosen chosen:
                return Choices.Matches(chosen.Characteristic, source is { } holder ? state.Object(holder) : null, obj, traits) == true;
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
