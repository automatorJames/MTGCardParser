namespace MTGEngine.Core;

/// <summary>A game object: a card or token in some zone, with the state that's per-object (tapped, damage, counters, attachment, who controls it).</summary>
public sealed record GameObject(
    ObjectId Id,
    CardDefinition Card,
    PlayerId Owner,
    PlayerId Controller,
    Zone Zone,
    bool Tapped = false,
    int Damage = 0,
    bool EnteredThisTurn = false,
    ImmutableDictionary<string, int> Counters = null,
    ObjectId? AttachedTo = null,
    bool Attacking = false,
    ObjectId? Blocking = null,
    bool FaceDown = false,
    int ZoneChangeCount = 0,
    ImmutableHashSet<string> CastWith = null,
    ImmutableDictionary<string, string> Choices = null,
    ManaPool SpentToCast = null,
    CardDefinition OriginalCard = null,
    int? CopyUntilTurn = null)
{
    /// <summary>How the object was cast, while it's the spell or the permanent that spell became: "Kicked", "AdditionalCost", an alternative cost's name.</summary>
    public bool WasCastWith(string how) => CastWith?.Contains(how) == true;

    /// <summary>A choice made for the object ("as this enters, choose a color"), by kind: "Color", "CreatureType", "CardName", …</summary>
    public string Chosen(string kind) => Choices?.GetValueOrDefault(kind);

    public ImmutableDictionary<string, int> CounterMap => Counters ?? ImmutableDictionary<string, int>.Empty;

    public int CountersOf(string kind) => CounterMap.GetValueOrDefault(kind);

    public bool IsToken => Card.IsToken;

    public bool OnBattlefield => Zone == Zone.Battlefield;

    public override string ToString() => $"{Card.Name}{Id}";
}

/// <summary>A spell or ability on the stack: who controls it, where it came from, what it does, what it targets, and X.</summary>
public sealed record StackItem(
    int Id,
    PlayerId Controller,
    ObjectId? Source,
    StackKind Kind,
    ImmutableList<Effect> Effects,
    ImmutableList<TargetRef> Targets,
    int X = 0,
    ObjectId? Spell = null,
    string Description = null,
    ImmutableList<Cost> Costs = null,
    TargetRef? Subject = null,
    int AbilityIndex = -1,
    ImmutableDictionary<string, ImmutableList<TargetRef>> Bindings = null,
    int? DelayedId = null)
{
    public override string ToString() => Description ?? (Spell is not null ? $"spell {Spell}" : $"ability of {Source}");
}

public enum StackKind { Spell, Activated, Triggered }

/// <summary>A pump or keyword grant that lasts until a turn ends (or forever, for one with no end).</summary>
public sealed record TemporaryEffect(ObjectId Target, int Power, int Toughness, ImmutableList<string> Keywords, int? UntilEndOfTurn, StatsMod BaseStats = null, bool LosesAbilities = false, ImmutableList<string> Restrictions = null, ImmutableList<Ability> Abilities = null, bool DoesntUntap = false, TypeChange TypeChange = null, ImmutableList<CombatRule> CombatRules = null,
    Condition While = null, ObjectId? WhileSource = null, PlayerId? WhileController = null, int WhileSourceZoneChanges = -1)
{
    /// <summary>Whether a lasting effect is in force: "for as long as you control this creature" holds, and "until this leaves the battlefield" hasn't happened.</summary>
    public bool ActiveIn(GameState state)
    {
        if (WhileSourceZoneChanges >= 0 && (WhileSource is not { } holder || state.Object(holder) is not { Zone: Zone.Battlefield } source || source.ZoneChangeCount != WhileSourceZoneChanges))
            return false;
        return While is null || Characteristics.Judging(() => Conditions.Evaluate(While, EffectContext.Snapshot(state, WhileController ?? default, WhileSource))) == true;
    }
}

/// <summary>"Target player can't cast instant or sorcery spells this turn", "you can't play lands": who, what, judged from whose side, and until when.</summary>
public sealed record PlayRestriction(PlayerId Player, string Verb, Selector What, ObjectId? Source, PlayerId Controller, int? ExpiresAfterTurn, bool OrActivate = false, Quantity MoreThan = null);

/// <summary>A card a permanent exiled, for "cards exiled with this artifact".</summary>
public sealed record ExileLink(ObjectId Source, int SourceZoneChanges, ObjectId Card, int CardZoneChanges);

/// <summary>A link from a permanent to what it exiled "until this leaves the battlefield".</summary>
public sealed record LinkedExile(ObjectId Source, int SourceZoneChanges, ImmutableList<(ObjectId Card, int ZoneChangeCount)> Exiled);

/// <summary>A trigger an effect set up for later: "when that creature dies this turn, …". One-shot; gone after it fires or its turn ends.</summary>
public sealed record DelayedTrigger(int Id, Event When, Effect Effect, PlayerId Controller, ObjectId? Source, TargetRef? Subject, int? ExpiresAfterTurn);

/// <summary>Mana in a pool that a restriction limits, with the object whose ability made it (for "spend this mana only to activate abilities of artifacts").</summary>
public sealed record RestrictedMana(Color? Color, int Amount, ManaRestriction Restriction, ObjectId Source, PlayerId Controller);

public sealed record PlayerState(
    PlayerId Id,
    string Name,
    int Life,
    ImmutableList<ObjectId> Library,
    ImmutableList<ObjectId> Hand,
    ImmutableList<ObjectId> Graveyard,
    ImmutableList<ObjectId> Exile,
    ManaPool Pool,
    int LandsPlayedThisTurn = 0,
    int ExtraLandsThisTurn = 0,
    bool Lost = false,
    int Poison = 0,
    bool DrewFromEmptyLibrary = false,
    int LifeGainedThisTurn = 0,
    int LifeLostThisTurn = 0,
    int SpellsCastThisTurn = 0,
    int Energy = 0,
    ImmutableList<RestrictedMana> Restricted = null,
    int Speed = 0,
    int SpeedRaisedOnTurn = 0)
{
    /// <summary>Mana in the pool that can be spent only on some things ("spend this mana only to cast creature spells"), apart from <see cref="Pool"/>.</summary>
    public ImmutableList<RestrictedMana> RestrictedList => Restricted ?? ImmutableList<RestrictedMana>.Empty;

    public ImmutableList<ObjectId> ZoneList(Zone zone) =>
        zone switch
        {
            Zone.Library => Library,
            Zone.Hand => Hand,
            Zone.Graveyard => Graveyard,
            Zone.Exile => Exile,
            _ => ImmutableList<ObjectId>.Empty,
        };

    public PlayerState WithZoneList(Zone zone, ImmutableList<ObjectId> list) =>
        zone switch
        {
            Zone.Library => this with { Library = list },
            Zone.Hand => this with { Hand = list },
            Zone.Graveyard => this with { Graveyard = list },
            Zone.Exile => this with { Exile = list },
            _ => this,
        };
}

/// <summary>
/// The whole state of a game, immutable: every change is a <see cref="GameEvent"/> applied by <see cref="Reducer"/>,
/// so the event log replays to the state and any state can be copied for lookahead by keeping a reference.
/// </summary>
public sealed record GameState(
    ImmutableArray<PlayerState> Players,
    ImmutableDictionary<ObjectId, GameObject> Objects,
    ImmutableList<StackItem> Stack,
    int Turn,
    PlayerId ActivePlayer,
    Step Step,
    PlayerId Priority,
    ImmutableList<TemporaryEffect> Temporary,
    int NextObjectId,
    int NextStackId,
    bool Over = false,
    PlayerId? Winner = null,
    int ConsecutivePasses = 0,
    bool CombatDamageDealt = false,
    ImmutableList<ObjectId> AttackedThisTurn = null,
    ImmutableList<ObjectId> DiedThisTurn = null,
    ImmutableDictionary<string, int> ActivationsThisTurn = null,
    ImmutableList<DelayedTrigger> Delayed = null,
    int NextDelayedId = 1,
    ImmutableList<PreventionShield> Shields = null,
    int NextShieldId = 1,
    ImmutableList<PlayPermission> Permissions = null,
    ImmutableList<ZoneReplacement> ZoneReplacements = null,
    ImmutableList<GameEvent> ThisTurnEvents = null,
    ImmutableList<GameEvent> LastTurnEvents = null,
    ImmutableList<LinkedExile> LinkedExiles = null,
    ImmutableList<ExileLink> ExileLinks = null,
    ImmutableList<PlayRestriction> PlayRestrictions = null,
    ImmutableList<(PlayerId Player, Step Step)> SkipNext = null,
    ImmutableList<ControlEffect> ControlEffects = null,
    ImmutableDictionary<ObjectId, GameObject> LastKnown = null)
{
    /// <summary>
    /// What a permanent was as it last left the battlefield, for "if it had a death counter on it" read after it died:
    /// last known information.
    /// </summary>
    public GameObject LastKnownOf(ObjectId id) => LastKnown?.GetValueOrDefault(id);

    /// <summary>The effects that change who controls permanents (see <see cref="ControlEffect"/>).</summary>
    public ImmutableList<ControlEffect> ControlEffectList => ControlEffects ?? ImmutableList<ControlEffect>.Empty;

    /// <summary>Steps players skip the next time they come: "you skip your next untap step".</summary>
    public ImmutableList<(PlayerId Player, Step Step)> SkipList => SkipNext ?? ImmutableList<(PlayerId, Step)>.Empty;

    public ImmutableList<ExileLink> ExileLinkList => ExileLinks ?? ImmutableList<ExileLink>.Empty;
    public ImmutableList<PlayRestriction> PlayRestrictionList => PlayRestrictions ?? ImmutableList<PlayRestriction>.Empty;

    public ImmutableList<LinkedExile> LinkedExileList => LinkedExiles ?? ImmutableList<LinkedExile>.Empty;

    /// <summary>What happened this turn and last turn, for "if a creature died this turn" (see <see cref="History"/>).</summary>
    public ImmutableList<GameEvent> ThisTurnEventList => ThisTurnEvents ?? ImmutableList<GameEvent>.Empty;
    public ImmutableList<GameEvent> LastTurnEventList => LastTurnEvents ?? ImmutableList<GameEvent>.Empty;

    public ImmutableList<ZoneReplacement> ZoneReplacementList => ZoneReplacements ?? ImmutableList<ZoneReplacement>.Empty;

    public ImmutableList<PlayPermission> PermissionList => Permissions ?? ImmutableList<PlayPermission>.Empty;

    public ImmutableList<PreventionShield> ShieldList => Shields ?? ImmutableList<PreventionShield>.Empty;

    public ImmutableDictionary<string, int> Activations => ActivationsThisTurn ?? ImmutableDictionary<string, int>.Empty;

    public ImmutableList<DelayedTrigger> DelayedTriggers => Delayed ?? ImmutableList<DelayedTrigger>.Empty;

    public PlayerState Player(PlayerId id) => Players[id.Value];

    public GameObject Object(ObjectId id) => Objects.TryGetValue(id, out var found) ? found : null;

    public IEnumerable<GameObject> Battlefield => Objects.Values.Where(x => x.Zone == Zone.Battlefield).OrderBy(x => x.Id.Value);

    /// <summary>Emblems: objects in the command zone, whose abilities work from there.</summary>
    public IEnumerable<GameObject> Emblems => Objects.Values.Where(x => x.Zone == Zone.Command && x.Card.Types.Contains("Emblem")).OrderBy(x => x.Id.Value);

    public IEnumerable<GameObject> InZone(Zone zone) => Objects.Values.Where(x => x.Zone == zone).OrderBy(x => x.Id.Value);

    public IEnumerable<GameObject> InZone(Zone zone, PlayerId player) =>
        zone is Zone.Battlefield or Zone.Stack
            ? InZone(zone).Where(x => x.Controller == player)
            : Player(player).ZoneList(zone).Select(Object).Where(x => x is not null);

    public IEnumerable<PlayerId> PlayerIds => Players.Select(x => x.Id);

    public IEnumerable<PlayerId> PlayersInGame => Players.Where(x => !x.Lost).Select(x => x.Id);

    public IEnumerable<PlayerId> OpponentsOf(PlayerId player) => PlayersInGame.Where(x => x != player);

    /// <summary>Players from <paramref name="first"/> around the table.</summary>
    public IEnumerable<PlayerId> TurnOrderFrom(PlayerId first) =>
        Enumerable.Range(0, Players.Length).Select(i => new PlayerId((first.Value + i) % Players.Length)).Where(x => !Player(x).Lost);

    public PlayerId NextInTurnOrder(PlayerId player) =>
        TurnOrderFrom(new PlayerId((player.Value + 1) % Players.Length)).First();

    public GameState WithPlayer(PlayerState player) => this with { Players = Players.SetItem(player.Id.Value, player) };

    public GameState WithObject(GameObject obj) => this with { Objects = Objects.SetItem(obj.Id, obj) };

    public bool IsMainPhase => Step is Step.Main1 or Step.Main2;

    public bool StackIsEmpty => Stack.Count == 0;
}
