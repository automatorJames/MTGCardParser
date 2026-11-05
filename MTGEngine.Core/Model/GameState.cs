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
    int ZoneChangeCount = 0)
{
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
    TargetRef? Subject = null)
{
    public override string ToString() => Description ?? (Spell is not null ? $"spell {Spell}" : $"ability of {Source}");
}

public enum StackKind { Spell, Activated, Triggered }

/// <summary>A pump or keyword grant that lasts until a turn ends (or forever, for one with no end).</summary>
public sealed record TemporaryEffect(ObjectId Target, int Power, int Toughness, ImmutableList<string> Keywords, int? UntilEndOfTurn, StatsMod BaseStats = null, bool LosesAbilities = false);

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
    bool Lost = false,
    int Poison = 0,
    bool DrewFromEmptyLibrary = false,
    int LifeGainedThisTurn = 0,
    int LifeLostThisTurn = 0,
    int SpellsCastThisTurn = 0)
{
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
    ImmutableList<ObjectId> DiedThisTurn = null)
{
    public PlayerState Player(PlayerId id) => Players[id.Value];

    public GameObject Object(ObjectId id) => Objects.TryGetValue(id, out var found) ? found : null;

    public IEnumerable<GameObject> Battlefield => Objects.Values.Where(x => x.Zone == Zone.Battlefield).OrderBy(x => x.Id.Value);

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
