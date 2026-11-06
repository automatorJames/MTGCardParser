namespace MTGEngine.Core;

/// <summary>Keyword actions as effects: scry, surveil, investigate, bolster, proliferate, explore, adapt, monstrosity, populate.</summary>
public static class KeywordActions
{
    public static readonly HashSet<string> Supported = ["Scry", "Surveil", "Investigate", "Bolster", "Proliferate", "Explore", "Adapt", "Monstrosity", "Populate", "Fateseal", "Connive", "Learn", "Clash", "Discover", "Manifest", "ManifestDread", "Cascade", "Detain", "Incubate", "Earthbend"];

    /// <summary>Earthbend's target, which the keyword names without a selector of its own: "target land you control".</summary>
    public static readonly ObjectSelector EarthbendTarget = new(Quantifier.Target, new ObjectFilter(Kinds: [new Term("Land", "CardType")], Controller: new Controlled(new PlayerSelector(PlayerKind.You))));

    /// <summary>What an earthbent land does when it dies or is exiled: returns to the battlefield tapped.</summary>
    static readonly TriggeredAbility _earthbentReturn = new(new ObjectEvent(new Self(), ["Dies", "IsPutIntoExileFromTheBattlefield"]),
        new ChangeZone(new Self(), new ZoneRef(nameof(Zone.Battlefield)), Tapped: true));

    public static EffectContext Execute(KeywordAction action, EffectContext ctx)
    {
        var game = ctx.Game;
        foreach (var keyword in action.Actions)
        {
            var amount = Quantities.Evaluate(keyword.Amount, ctx) ?? 1;
            switch (keyword.Keyword)
            {
                case "Scry":
                    Scry(game, ctx.Controller, amount, toGraveyard: false);
                    break;
                case "Surveil":
                    Scry(game, ctx.Controller, amount, toGraveyard: true);
                    break;
                // Incubate N: an Incubator artifact token with N +1/+1 counters and "{2}: transform this artifact", whose back is a 0/0 Phyrexian artifact creature.
                case "Incubate":
                {
                    var hatch = new ActivatedAbility([new PayMana(["{2}"])], new Animate(new Self(), new Fixed(0), new Fixed(0), Subtypes: ["Phyrexian"], Types: ["Artifact", "Creature"]));
                    var incubator = game.CreateToken(new TokenSpec(Types: ["Artifact"], Subtypes: ["Incubator"], Abilities: [hatch]), ctx.Controller);
                    if (amount > 0)
                        game.Emit(new CountersChanged(incubator, "+1/+1", amount));
                    ctx = ctx with { Last = [TargetRef.Of(incubator)], Affected = [TargetRef.Of(incubator)] };
                    break;
                }
                case "Investigate":
                    for (var i = 0; i < amount; i++)
                        game.CreateToken(new TokenSpec(Types: ["Artifact"], Subtypes: ["Clue"], Predefined: "Clue"), ctx.Controller);
                    break;
                case "Bolster":
                {
                    var mine = ctx.State.Battlefield.Where(x => x.Controller == ctx.Controller && Characteristics.Of(ctx.State, x).IsCreature).ToList();
                    if (mine.Count == 0)
                        break;
                    var least = mine.Min(x => Characteristics.Of(ctx.State, x).Toughness);
                    var candidates = mine.Where(x => Characteristics.Of(ctx.State, x).Toughness == least).Select(x => TargetRef.Of(x.Id)).ToImmutableList();
                    var chosen = candidates.Count == 1 ? candidates[0] : game.Decide(new ChooseTargets(ctx.Controller, "Bolster", candidates, 1, 1)).Targets?.FirstOrDefault() ?? candidates[0];
                    if (chosen.IsObject)
                        game.Emit(new CountersChanged(chosen.Object.Value, "+1/+1", amount));
                    break;
                }
                case "Proliferate":
                {
                    foreach (var obj in ctx.State.Battlefield.Where(x => x.CounterMap.Count > 0).ToList())
                        foreach (var kind in obj.CounterMap.Keys.Where(k => k != "deathtouch-damage").ToList())
                            if (obj.Controller == ctx.Controller || kind == "-1/-1")
                                game.Emit(new CountersChanged(obj.Id, kind, 1));
                    foreach (var player in ctx.State.PlayersInGame.Where(p => ctx.State.Player(p).Poison > 0 && p != ctx.Controller).ToList())
                        game.Emit(new PoisonChanged(player, 1));
                    break;
                }
                case "Explore":
                {
                    var explorer = ctx.Source is { } s && ctx.State.Object(s) is { Zone: Zone.Battlefield } ? s : (ctx.Last?.FirstOrDefault(x => x.IsObject).Object);
                    var library = ctx.State.Player(ctx.Controller).Library;
                    if (library.Count == 0)
                        break;
                    var top = library[0];
                    if (ctx.State.Object(top).Card.IsLand)
                        game.Emit(new Drew(ctx.Controller, top));
                    else
                    {
                        if (explorer is { } id)
                            game.Emit(new CountersChanged(id, "+1/+1", 1));
                        if (game.Decide(new YesNo(ctx.Controller, "Explore: put the card into your graveyard?")).Yes)
                            game.MoveToZone(top, Zone.Graveyard);
                    }
                    break;
                }
                case "Adapt":
                    if (ctx.Source is { } adapting && ctx.State.Object(adapting) is { } creature && creature.CountersOf("+1/+1") == 0)
                        game.Emit(new CountersChanged(adapting, "+1/+1", amount));
                    break;
                // Earthbend N: target land you control becomes a 0/0 creature with haste that's still a land, with N +1/+1
                // counters; when it dies or is exiled, it returns to the battlefield tapped.
                case "Earthbend":
                {
                    if (!ctx.TargetsBySelector.TryGetValue(EarthbendTarget, out var lands) || lands.FirstOrDefault(x => x.IsObject) is not { IsObject: true } land
                        || ctx.State.Object(land.Object.Value) is not { Zone: Zone.Battlefield })
                        break;
                    var id = land.Object.Value;
                    Interpreter.Execute(new Animate(new Self(), new Fixed(0), new Fixed(0), Types: ["Creature"], Keywords: [new KeywordInstance("Haste")], InAddition: true), ctx with { Source = id });
                    game.Emit(new TemporaryEffectAdded(new TemporaryEffect(id, 0, 0, [], null, Abilities: [_earthbentReturn])));
                    if (amount > 0)
                        game.Emit(new CountersChanged(id, "+1/+1", amount));
                    ctx = ctx with { Last = [land], Affected = [land] };
                    break;
                }
                // Detain: until your next turn, the permanents can't attack or block and their activated abilities can't be activated.
                case "Detain" when action.Objects is { } detained:
                {
                    var until = Interpreter.UntilTurn(new UntilPhase(new PhaseRef("Turn", Part: "End", Whose: new PlayerSelector(PlayerKind.You))), ctx.State, ctx.Controller) - 1;
                    foreach (var id in Resolver.Objects(detained, ctx))
                        game.Emit(new TemporaryEffectAdded(new TemporaryEffect(id, 0, 0, [], until, Restrictions: ["Attack", "Block", "Activate"])));
                    break;
                }
                case "Monstrosity":
                    if (ctx.Source is { } monster && ctx.State.Object(monster) is { } beast && beast.CountersOf("monstrous") == 0)
                    {
                        game.Emit(new CountersChanged(monster, "+1/+1", amount));
                        game.Emit(new CountersChanged(monster, "monstrous", 1));
                    }
                    break;
                case "Populate":
                {
                    var tokens = ctx.State.Battlefield.Where(x => x.Controller == ctx.Controller && x.IsToken && Characteristics.Of(ctx.State, x).IsCreature).Select(x => TargetRef.Of(x.Id)).ToImmutableList();
                    if (tokens.Count == 0)
                        break;
                    var chosen = tokens.Count == 1 ? tokens[0] : game.Decide(new ChooseTargets(ctx.Controller, "Populate", tokens, 1, 1)).Targets?.FirstOrDefault() ?? tokens[0];
                    if (chosen.IsObject && ctx.State.Object(chosen.Object.Value) is { } original)
                        game.CreateCopyToken(original.Card, ctx.Controller);
                    break;
                }
                case "Fateseal":
                {
                    foreach (var opponent in ctx.State.OpponentsOf(ctx.Controller).ToList())
                        Scry(game, opponent, amount, toGraveyard: false, chooser: ctx.Controller);
                    break;
                }
                // Clash: each clashing player reveals their top card and may put it on the bottom; you win with the greater mana value.
                case "Clash":
                {
                    var opponents = ctx.State.OpponentsOf(ctx.Controller).ToList();
                    if (opponents.Count == 0)
                        break;
                    var opponent = opponents.Count == 1 ? opponents[0] : opponents[Math.Clamp(game.Decide(new ChooseOne(ctx.Controller, "Clash with", opponents.Select(x => x.ToString()).ToImmutableList())).Index, 0, opponents.Count - 1)];
                    int Reveal(PlayerId player)
                    {
                        if (ctx.State.Player(player).Library is not [var top, ..])
                            return -1;
                        var card = ctx.State.Object(top);
                        if (game.Decide(new YesNo(player, $"Clash: put {card.Card.Name} on the bottom of your library?")).Yes)
                            game.MoveToZone(top, Zone.Library, position: int.MaxValue);
                        return card.Card.ManaCost.ManaValue;
                    }
                    var mine = Reveal(ctx.Controller);
                    var theirs = Reveal(opponent);
                    game.WonLastFlip = mine > theirs;
                    game.LastOptionalTaken = game.WonLastFlip;
                    break;
                }
                // Discover N: exile from the top until a nonland card with mana value N or less; cast it free or put it into
                // your hand; the rest go on the bottom in a random order.
                // Cascade: the same, for a nonland card that costs less than the spell; one not cast goes on the bottom too.
                case "Discover":
                case "Cascade":
                {
                    List<ObjectId> exiled = [];
                    ObjectId? found = null;
                    while (ctx.State.Player(ctx.Controller).Library is [var top, ..])
                    {
                        game.MoveToZone(top, Zone.Exile);
                        if (ctx.State.Object(top) is { } card && !card.Card.IsLand && card.Card.ManaCost.ManaValue <= amount)
                        {
                            found = top;
                            break;
                        }
                        exiled.Add(top);
                    }
                    if (found is { } discovered)
                    {
                        if (!(game.Decide(new YesNo(ctx.Controller, $"{keyword.Keyword}: cast {ctx.State.Object(discovered).Card.Name} without paying its mana cost?")).Yes && game.CastFree(discovered, ctx.Controller)))
                        {
                            if (keyword.Keyword == "Cascade")
                                exiled.Add(discovered);
                            else
                                game.MoveToZone(discovered, Zone.Hand);
                        }
                    }
                    foreach (var id in exiled.OrderBy(_ => game.Rng.Next()).ToList())
                        game.MoveToZone(id, Zone.Library, position: int.MaxValue);
                    break;
                }
                // Manifest: the top card of your library (or what's named) onto the battlefield face down as a 2/2 creature.
                case "Manifest":
                {
                    var cards = action.Objects is { } named ? Resolver.Objects(named, ctx) : ctx.State.Player(ctx.Controller).Library.Take(amount).ToImmutableList();
                    List<TargetRef> manifested = [];
                    foreach (var card in cards.ToList())
                        if (game.Manifest(card, ctx.Controller) is { } id)
                            manifested.Add(TargetRef.Of(id));
                    ctx = ctx with { Last = manifested.ToImmutableList(), Affected = manifested.ToImmutableList() };
                    break;
                }
                // Manifest dread: look at the top two cards of your library, manifest one and put the other into your graveyard.
                case "ManifestDread":
                {
                    var top = ctx.State.Player(ctx.Controller).Library.Take(2).ToImmutableList();
                    if (top.Count == 0)
                        break;
                    var choice = top.Count == 1 ? null : game.Decide(new ChooseTargets(ctx.Controller, "Manifest dread: the card to manifest", top.Select(TargetRef.Of).ToImmutableList(), 1, 1)).Targets;
                    var pick = choice?.Where(x => x.IsObject && top.Contains(x.Object.Value)).Select(x => x.Object.Value).FirstOrDefault() is { } chosen && top.Contains(chosen) ? chosen : top[0];
                    foreach (var other in top.Where(x => x != pick).ToList())
                        game.MoveToZone(other, Zone.Graveyard);
                    if (game.Manifest(pick, ctx.Controller) is { } dread)
                        ctx = ctx with { Last = [TargetRef.Of(dread)], Affected = [TargetRef.Of(dread)] };
                    break;
                }
                // Learn: there's no sideboard to get a lesson from, so it's "you may discard a card. if you do, draw a card".
                case "Learn":
                {
                    var hand = ctx.State.Player(ctx.Controller).Hand;
                    if (hand.Count > 0 && game.Decide(new YesNo(ctx.Controller, "Learn: discard a card to draw a card?")).Yes)
                    {
                        var pick = game.Decide(new ChooseTargets(ctx.Controller, "Learn: discard", hand.Select(TargetRef.Of).ToImmutableList(), 1, 1)).Targets?.FirstOrDefault(x => x.IsObject && hand.Contains(x.Object.Value));
                        game.Discard(ctx.Controller, pick?.Object ?? hand[0]);
                        game.Draw(ctx.Controller, 1);
                    }
                    break;
                }
                // Connive: draw a card, then discard a card; if a nonland card was discarded, the conniver gets a +1/+1 counter.
                case "Connive":
                {
                    var connivers = action.Objects is { } who ? Resolver.Objects(who, ctx) : ctx.Source is { } own ? [own] : [];
                    foreach (var conniver in connivers.ToList())
                    {
                        var player = ctx.State.Object(conniver)?.Controller ?? ctx.Controller;
                        game.Draw(player, amount);
                        var hand = ctx.State.Player(player).Hand;
                        if (hand.Count == 0)
                            continue;
                        var take = Math.Min(amount, hand.Count);
                        var discarded = (take >= hand.Count ? hand : game.Decide(new ChooseTargets(player, "Connive: discard", hand.Select(TargetRef.Of).ToImmutableList(), take, take))
                            .Targets?.Where(x => x.IsObject && hand.Contains(x.Object.Value)).Select(x => x.Object.Value).Take(take) ?? hand.Take(take)).ToList();
                        var nonland = discarded.Count(id => ctx.State.Object(id) is { } card && !card.Card.IsLand);
                        foreach (var id in discarded)
                            game.Discard(player, id);
                        if (nonland > 0 && ctx.State.Object(conniver) is { Zone: Zone.Battlefield })
                            game.Emit(new CountersChanged(conniver, "+1/+1", nonland));
                    }
                    break;
                }
                default:
                    game.Skip(keyword, $"keyword action {keyword.Keyword}");
                    break;
            }
        }
        return ctx;
    }

    static void Scry(Game game, PlayerId player, int count, bool toGraveyard, PlayerId? chooser = null)
    {
        var library = game.State.Player(player).Library;
        var top = library.Take(count).ToImmutableList();
        if (top.Count == 0)
            return;
        var away = game.Decide(new ChooseTargets(chooser ?? player, toGraveyard ? "Surveil: cards to put into your graveyard" : "Scry: cards to put on the bottom", top.Select(TargetRef.Of).ToImmutableList(), 0, top.Count))
            .Targets?.Where(x => x.IsObject).Select(x => x.Object.Value).Where(top.Contains).Distinct().ToImmutableList() ?? [];
        var kept = top.Where(x => !away.Contains(x)).ToImmutableList();
        game.Emit(new Scryed(player, kept, away));
        foreach (var id in away)
            game.MoveToZone(id, toGraveyard ? Zone.Graveyard : Zone.Library, position: int.MaxValue);
    }
}
