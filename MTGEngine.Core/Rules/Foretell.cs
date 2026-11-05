namespace MTGEngine.Core;

/// <summary>
/// Foretell: during its owner's turn, a card with foretell may be exiled from the hand face down for {2} (a special
/// action, any time they have priority); on a later turn it may be cast from exile for its foretell cost.
/// </summary>
public sealed partial class Game
{
    /// <summary>The alternative index that marks foretelling a card (exiling it face down) rather than casting it.</summary>
    public const int ForetellExile = -6;

    /// <summary>The alternative index that marks casting a foretold card for its foretell cost.</summary>
    const int ForetoldCast = -7;

    static readonly ManaCost _foretellFee = ManaCost.Parse("{2}");

    static Cost ForetellCost(CardDefinition card) => KeywordCost(card, "Foretell");

    /// <summary>Whether <paramref name="player"/> could foretell <paramref name="obj"/> now: in their hand, on their turn, with {2}.</summary>
    bool CanForetell(GameObject obj, PlayerId player) =>
        obj.Zone == Zone.Hand && State.ActivePlayer == player && ForetellCost(obj.Card) is PayMana && ManaPayment.CanPay(State, player, _foretellFee);

    void Foretell(ObjectId id, PlayerId player)
    {
        if (State.Object(id) is not { Zone: Zone.Hand } obj || !CanForetell(obj, player) || !Pay(player, _foretellFee))
            return;
        MoveToZone(id, Zone.Exile);
        Emit(new TurnedFace(id, FaceDown: true));
        Emit(new ChoiceRecorded(id, "ForetoldTurn", State.Turn.ToString()));
    }

    /// <summary>Whether <paramref name="obj"/> is a foretold card <paramref name="player"/> may cast now: exiled face down by foretelling on an earlier turn.</summary>
    bool CanCastForetold(GameObject obj, PlayerId player, bool sorcerySpeed) =>
        obj is { Zone: Zone.Exile, FaceDown: true } && obj.Owner == player && int.TryParse(obj.Chosen("ForetoldTurn"), out var turn) && turn < State.Turn
        && ForetellCost(obj.Card) is { } cost && (sorcerySpeed || obj.Card.IsInstant || obj.Card.Keywords.Contains("Flash"))
        && AdditionalPayable(cost, player, obj.Id, 0) && TargetsAvailable(SpellSlots(obj.Card), player, obj.Id);

    /// <summary>The foretell actions <paramref name="player"/> has now: foretelling cards in hand, and casting cards foretold earlier.</summary>
    IEnumerable<LegalAction> ForetellActions(PlayerId player, bool sorcerySpeed)
    {
        foreach (var obj in State.Player(player).Hand.Select(State.Object).Where(x => x is not null && CanForetell(x, player)))
            yield return new CastSpell(obj.Id, 0, Alternative: ForetellExile);
        foreach (var obj in State.Objects.Values.Where(x => CanCastForetold(x, player, sorcerySpeed)).ToList())
            yield return new CastSpell(obj.Id, 0, Alternative: ForetoldCast);
    }
}
