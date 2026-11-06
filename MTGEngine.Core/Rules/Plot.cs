namespace MTGEngine.Core;

/// <summary>
/// Casting keywords that are a cost paid instead of the mana cost: evoke and dash cast from the hand for that cost
/// (the abilities they add are in <see cref="KeywordAbilities"/>); plot exiles the card from the hand for its cost
/// at sorcery speed (a special action), and on a later turn it may be cast as a sorcery without paying its mana cost.
/// </summary>
public sealed partial class Game
{
    /// <summary>The alternative index that marks casting for an evoke cost.</summary>
    const int EvokeCast = -8;

    /// <summary>The alternative index that marks casting for a dash cost.</summary>
    const int DashCast = -9;

    /// <summary>The alternative index that marks plotting a card (exiling it) rather than casting it.</summary>
    public const int PlotExile = -10;

    /// <summary>The alternative index that marks casting a plotted card.</summary>
    const int PlottedCast = -11;

    /// <summary>The alternative index that marks casting for a warp cost.</summary>
    const int WarpCast = -12;

    /// <summary>The alternative index that marks casting for an awaken cost.</summary>
    const int AwakenCast = -13;

    /// <summary>The casting keywords that replace the mana cost when cast from the hand, with their indices and the option a spell cast that way records.</summary>
    static readonly (string Keyword, int Index, string Option)[] _keywordCasts =
        [("Evoke", EvokeCast, "Evoked"), ("Dash", DashCast, "Dashed"), ("Warp", WarpCast, "Warped"), ("Awaken", AwakenCast, "Awakened")];

    /// <summary>Exiles a warped permanent at the end step; its owner may cast it from exile on a later turn.</summary>
    public void ExileForLater(ObjectId id)
    {
        if (State.Object(id) is not { Zone: Zone.Battlefield })
            return;
        MoveToZone(id, Zone.Exile);
        if (State.Object(id) is { Zone: Zone.Exile })
            Emit(new ChoiceRecorded(id, "CastableFromExileAfter", State.Turn.ToString()));
    }

    /// <summary>Whether <paramref name="obj"/> is a card exiled by warp on an earlier turn that <paramref name="player"/> may cast now, as they could from the hand.</summary>
    bool CanCastWarped(GameObject obj, PlayerId player, bool sorcerySpeed) =>
        obj is { Zone: Zone.Exile } && obj.Owner == player && int.TryParse(obj.Chosen("CastableFromExileAfter"), out var turn) && turn < State.Turn
        && (sorcerySpeed || obj.Card.IsInstant || obj.Card.Keywords.Contains("Flash")) && CanAffordToCast(obj, player) && TargetsAvailable(SpellSlots(obj.Card), player, obj.Id);

    /// <summary>The ways <paramref name="obj"/> in the hand could be cast for a keyword's cost now.</summary>
    IEnumerable<LegalAction> KeywordCastActions(GameObject obj, PlayerId player)
    {
        foreach (var (keyword, index, _) in _keywordCasts)
            if (KeywordCost(obj.Card, keyword) is { } cost && AdditionalPayable(cost, player, obj.Id, 0) && KeywordCastPayable(obj, player, cost))
                yield return new CastSpell(obj.Id, 0, Alternative: index);
    }

    bool KeywordCastPayable(GameObject obj, PlayerId player, Cost cost) =>
        (cost is not PayMana mana || ManaPayment.CanPay(State, player, ManaCost.Parse(string.Concat(mana.Symbols)).Plus(RequiredAdditionalMana(obj.Card)), HasConvoke(obj.Card), new ManaPayment.Purpose(Spell: obj)))
        && AdditionalCostsOf(obj.Card).All(c => AdditionalPayable(c, player, obj.Id, 0));

    /// <summary>The keyword cost and option for a keyword alternative index; null for other indices.</summary>
    static (Cost Cost, string Option)? KeywordCast(CardDefinition card, int alternative) =>
        _keywordCasts.Where(x => x.Index == alternative).Select(x => KeywordCost(card, x.Keyword) is { } cost ? (cost, x.Option) : ((Cost, string)?)null).FirstOrDefault();

    static Cost PlotCost(CardDefinition card) => KeywordCost(card, "Plot");

    /// <summary>Whether <paramref name="player"/> could plot <paramref name="obj"/> now: in their hand, at sorcery speed, with its plot cost.</summary>
    bool CanPlot(GameObject obj, PlayerId player, bool sorcerySpeed) =>
        sorcerySpeed && obj.Zone == Zone.Hand && PlotCost(obj.Card) is { } cost && AdditionalPayable(cost, player, obj.Id, 0);

    void Plot(ObjectId id, PlayerId player)
    {
        if (State.Object(id) is not { Zone: Zone.Hand } obj || PlotCost(obj.Card) is not { } cost || !AdditionalPayable(cost, player, id, 0))
            return;
        var mana = ManaCost.Free;
        List<Effect> actions = [];
        if (!PlanAdditional(cost, player, id, 0, ref mana, actions, ImmutableHashSet.CreateBuilder<string>()) || !Pay(player, mana))
            return;
        var ctx = new EffectContext(this, player, id, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
        foreach (var action in actions)
            ctx = Interpreter.Execute(action, ctx);
        MoveToZone(id, Zone.Exile);
        Emit(new ChoiceRecorded(id, "PlottedTurn", State.Turn.ToString()));
    }

    /// <summary>Whether <paramref name="obj"/> is a card <paramref name="player"/> plotted on an earlier turn and may cast now (as a sorcery, without paying its mana cost).</summary>
    bool CanCastPlotted(GameObject obj, PlayerId player, bool sorcerySpeed) =>
        sorcerySpeed && obj is { Zone: Zone.Exile } && obj.Owner == player && int.TryParse(obj.Chosen("PlottedTurn"), out var turn) && turn < State.Turn
        && !obj.Card.IsLand && AdditionalCostsOf(obj.Card).All(cost => AdditionalPayable(cost, player, obj.Id, 0)) && TargetsAvailable(SpellSlots(obj.Card), player, obj.Id);

    /// <summary>The plot actions <paramref name="player"/> has now: plotting cards in hand, and casting cards plotted earlier.</summary>
    IEnumerable<LegalAction> PlotActions(PlayerId player, bool sorcerySpeed)
    {
        foreach (var obj in State.Objects.Values.Where(x => CanCastWarped(x, player, sorcerySpeed)).ToList())
            yield return new CastSpell(obj.Id, 0);
        if (!sorcerySpeed)
            yield break;
        foreach (var obj in State.Player(player).Hand.Select(State.Object).Where(x => x is not null && CanPlot(x, player, sorcerySpeed)).ToList())
            yield return new CastSpell(obj.Id, 0, Alternative: PlotExile);
        foreach (var obj in State.Objects.Values.Where(x => CanCastPlotted(x, player, sorcerySpeed)).ToList())
            yield return new CastSpell(obj.Id, 0, true, Alternative: PlottedCast);
    }
}
