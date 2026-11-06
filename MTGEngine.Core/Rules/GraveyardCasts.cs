namespace MTGEngine.Core;

/// <summary>
/// Casting keywords that work from the graveyard: escape casts the card for its escape cost (often mana and
/// exiling other cards from the graveyard), and retrace casts it for its mana cost and discarding a land card;
/// unlike flashback, the card goes where it would after resolving, so it may be cast again. Jump-start casts it for
/// its mana cost and discarding a card, and exiles it as it leaves the stack. Sneak casts a card from the hand during
/// the declare blockers step for its sneak cost and returning an unblocked attacker; surge and spectacle are costs
/// from the hand once you've cast another spell, or an opponent has lost life, this turn.
/// </summary>
public sealed partial class Game
{
    /// <summary>The alternative index that marks casting for an escape cost.</summary>
    const int EscapeCast = -16;

    /// <summary>The alternative index that marks casting with retrace.</summary>
    const int RetraceCast = -17;

    /// <summary>The alternative index that marks casting for a sneak cost.</summary>
    const int SneakCast = -18;

    /// <summary>The alternative index that marks casting with jump-start.</summary>
    const int JumpStartCast = -19;

    const int SurgeCast = -20;
    const int SpectacleCast = -21;
    const int BlitzCast = -22;

    /// <summary>Keyword costs paid instead of the mana cost that aren't offered from the hand at the usual times (see <see cref="KeywordCast"/>).</summary>
    static readonly (string Keyword, int Index, string Option)[] _otherZoneCasts = [("Escape", EscapeCast, "Escaped"), ("Sneak", SneakCast, "Sneak")];

    static readonly Cost _jumpStartCost = new ActionCost(new Discard(new PlayerSelector(PlayerKind.You), new Fixed(1)));

    /// <summary>Sneak's other cost: returning an unblocked attacker you control to its owner's hand.</summary>
    static readonly Cost _sneakReturn = new ActionCost(new ChangeZone(
        new ObjectSelector(Quantifier.Any, new ObjectFilter(Kinds: [new Term("Creature", "CardType")], Qualifiers: [new Qualifier("Unblocked", Vocabulary: "CombatState")], Controller: new Controlled(new PlayerSelector(PlayerKind.You)))),
        new ZoneRef(nameof(Zone.Hand))));

    /// <summary>Whether a keyword cost's condition holds: surge once you've cast another spell this turn, spectacle once an opponent has lost life.</summary>
    bool KeywordCastAllowed(int index, PlayerId player) =>
        index switch
        {
            SurgeCast => State.ThisTurnEventList.OfType<SpellCast>().Any(x => x.Item.Controller == player),
            SpectacleCast => State.ThisTurnEventList.OfType<LifeChanged>().Any(x => x.Delta < 0 && x.Player != player),
            _ => true,
        };

    /// <summary>The cards <paramref name="player"/> could cast for their sneak cost now: on their turn, once blockers are declared, with an unblocked attacker.</summary>
    IEnumerable<LegalAction> SneakActions(PlayerId player)
    {
        if (State.Step != Step.DeclareBlockers || State.ActivePlayer != player || !AdditionalPayable(_sneakReturn, player, default, 0))
            yield break;
        foreach (var obj in State.Player(player).Hand.Select(State.Object).Where(x => x is not null).ToList())
            if (KeywordCost(obj.Card, "Sneak") is { } cost && !Forbidden(player, obj) && AdditionalPayable(cost, player, obj.Id, 0) && KeywordCastPayable(obj, player, cost)
                && TargetsAvailable(SpellSlots(obj.Card), player, obj.Id))
                yield return new CastSpell(obj.Id, 0, Alternative: SneakCast);
    }

    static readonly Cost _retraceCost = new ActionCost(new Discard(new PlayerSelector(PlayerKind.You), new Fixed(1),
        new ObjectFilter(Kinds: [new Term(nameof(ObjectKind.Card), nameof(ObjectKind))], Qualifiers: [new Qualifier("Land", Vocabulary: "CardType")])));

    /// <summary>The ways <paramref name="obj"/> in the graveyard could be cast by escape or retrace now.</summary>
    IEnumerable<LegalAction> GraveyardCastActions(GameObject obj, PlayerId player, bool sorcerySpeed)
    {
        if (obj.Zone != Zone.Graveyard || obj.Owner != player || Forbidden(player, obj) || !(sorcerySpeed || obj.Card.IsInstant || obj.Card.Keywords.Contains("Flash"))
            || !TargetsAvailable(SpellSlots(obj.Card), player, obj.Id))
            yield break;
        if (KeywordCost(obj.Card, "Escape") is { } escape && AdditionalPayable(escape, player, obj.Id, 0) && KeywordCastPayable(obj, player, escape))
            yield return new CastSpell(obj.Id, 0, Alternative: EscapeCast);
        if (obj.Card.Keywords.Contains("Retrace") && CanAffordToCast(obj, player) && AdditionalPayable(_retraceCost, player, obj.Id, 0))
            yield return new CastSpell(obj.Id, 0, Alternative: RetraceCast);
        if (obj.Card.Keywords.Contains("JumpStart") && CanAffordToCast(obj, player) && AdditionalPayable(_jumpStartCost, player, obj.Id, 0))
            yield return new CastSpell(obj.Id, 0, Alternative: JumpStartCast);
    }

    /// <summary>Split second: while a spell with it is on the stack, players can't cast spells or activate abilities other than mana abilities.</summary>
    bool SplitSecond() =>
        State.Stack.Any(x => x.Kind == StackKind.Spell && (x.Spell ?? x.Source) is { } id && State.Object(id) is { } spell && spell.Card.Keywords.Contains("SplitSecond"));

    bool IsManaAbility(ActivateAbility activate) =>
        State.Object(activate.Source) is { } obj && Characteristics.AbilitiesOf(State, obj).ElementAtOrDefault(activate.Index) is ActivatedAbility { Effect: AddMana or LetVariable { Effect: AddMana } };

    /// <summary>Totem armor: if a creature an aura with it enchants would be destroyed, its damage is removed and the aura is destroyed instead.</summary>
    bool TotemArmorSaves(ObjectId creature)
    {
        if (State.Battlefield.FirstOrDefault(x => x.AttachedTo == creature && Characteristics.Of(State, x).HasKeyword("TotemArmor")) is not { } umbra)
            return false;
        Emit(new DamageRemovedFrom(creature));
        Destroy(umbra.Id);
        return true;
    }
}
