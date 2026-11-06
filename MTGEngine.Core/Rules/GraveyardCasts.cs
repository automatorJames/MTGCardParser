namespace MTGEngine.Core;

/// <summary>
/// Casting keywords that work from the graveyard: escape casts the card for its escape cost (often mana and
/// exiling other cards from the graveyard), and retrace casts it for its mana cost and discarding a land card.
/// Unlike flashback, the card goes where it would after resolving, so it may be cast again.
/// </summary>
public sealed partial class Game
{
    /// <summary>The alternative index that marks casting for an escape cost.</summary>
    const int EscapeCast = -16;

    /// <summary>The alternative index that marks casting with retrace.</summary>
    const int RetraceCast = -17;

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
