namespace MTGEngine.Core;

/// <summary>
/// Suspend: rather than cast a card from the hand, its owner may pay its suspend cost and exile it with that many time
/// counters. A suspended card is one in exile with suspend and time counters; at the beginning of its owner's upkeep
/// one is removed, and when the last is, its owner may cast it without paying its mana cost - a creature cast so has
/// haste.
/// </summary>
public sealed partial class Game
{
    /// <summary>The alternative index that marks suspending a card rather than casting it.</summary>
    public const int SuspendCast = -5;

    /// <summary>A card's suspend keyword, if it has one the engine can carry out: a fixed number of time counters and a mana cost.</summary>
    static KeywordInstance SuspendKeyword(CardDefinition card) =>
        card.Abilities.OfType<KeywordLine>().SelectMany(x => x.Keywords).FirstOrDefault(x => CardDefinition.Normalize(x.Keyword) == "Suspend" && Suspendable(x));

    /// <summary>Whether the engine carries out a suspend keyword: "suspend 4—{u}", not "suspend x—{x}{3}{u}".</summary>
    public static bool Suspendable(KeywordInstance keyword) =>
        keyword is { Amount: Fixed { Value: > 0 }, Cost: PayMana mana } && !mana.Symbols.Any(x => x.Contains('x', StringComparison.OrdinalIgnoreCase));

    static ManaCost SuspendCost(KeywordInstance keyword) => ManaCost.Parse(string.Concat(((PayMana)keyword.Cost).Symbols));

    /// <summary>Whether <paramref name="player"/> could suspend <paramref name="obj"/> from their hand now, timed as if casting it.</summary>
    bool CanSuspend(GameObject obj, PlayerId player, bool sorcerySpeed) =>
        obj.Zone == Zone.Hand && SuspendKeyword(obj.Card) is { } keyword
        && (sorcerySpeed || obj.Card.IsInstant || obj.Card.Keywords.Contains("Flash"))
        && ManaPayment.CanPay(State, player, SuspendCost(keyword));

    /// <summary>Suspends a card: pays its suspend cost and exiles it from the hand with its time counters.</summary>
    void Suspend(ObjectId id, PlayerId player)
    {
        if (State.Object(id) is not { Zone: Zone.Hand } obj || SuspendKeyword(obj.Card) is not { Amount: Fixed { Value: var count } } keyword)
            return;
        if (!Pay(player, SuspendCost(keyword)))
            return;
        MoveToZone(id, Zone.Exile);
        Emit(new CountersChanged(id, "time", count));
    }

    /// <summary>The upkeep's suspend step for <paramref name="active"/>'s suspended cards: a time counter off each, and casting those whose last is removed.</summary>
    void TickSuspended(PlayerId active)
    {
        foreach (var obj in State.Objects.Values.Where(x => x.Owner == active && x.Zone == Zone.Exile && x.CountersOf("time") > 0 && SuspendKeyword(x.Card) is not null).ToList())
        {
            Emit(new CountersChanged(obj.Id, "time", -1));
            if (State.Object(obj.Id) is not { Zone: Zone.Exile } card || card.CountersOf("time") > 0)
                continue;
            if (!TargetsAvailable(SpellSlots(card.Card), active, card.Id) || !Decide(new YesNo(active, $"Cast {card.Card.Name} without paying its mana cost?")).Yes)
                continue;
            Cast(card.Id, active, 0, free: true, also: "Suspended");
        }
    }
}
