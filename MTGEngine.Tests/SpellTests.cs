using MTGEngine.Agents;

namespace MTGEngine.Tests;

/// <summary>Real cards, from their lowered rules, played by a greedy agent against a passive one; the opening hand is the top of the (unshuffled) deck.</summary>
public sealed class SpellTests
{
    static Deck Hand(string player, params CardDefinition[] cards) =>
        new(player, [.. cards, .. Enumerable.Repeat(Cards.Basic("Plains"), 20)]);

    static Deck Lands(string player, string land = "Plains") => Cards.DeckOf(player, (Cards.Basic(land), 20));

    static ObjectId Put(Game game, CardDefinition card, int player, bool sick = false)
    {
        var id = new ObjectId(game.State.NextObjectId);
        var owner = new PlayerId(player);
        game.Emit(new ObjectCreated(new GameObject(id, card, owner, owner, Zone.Hand)));
        game.MoveToZone(id, Zone.Battlefield, owner);
        if (!sick)
            game.Emit(new SummoningSicknessCleared(owner));
        return id;
    }

    [Fact]
    public void LightningBoltAtTheFaceDealsThree()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Mountain"), Cards.Named("Lightning Bolt")), Lands("B"), new GreedyAgent(attacks: false));

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(17, game.P1().Life);
        Assert.Equal(Zone.Graveyard, game.Find("Lightning Bolt").Zone);
        Assert.Contains(game.Log, x => x is SpellCast);
        Assert.Empty(game.Skips);
    }

    [Fact]
    public void ShockKillsATwoToughnessCreature()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Mountain"), Cards.Named("Shock")), Lands("B"), new GreedyAgent(attacks: false));
        var bears = Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(bears).Zone);
        Assert.Equal(20, game.P1().Life);
    }

    [Fact]
    public void GiantGrowthPumpsUntilEndOfTurn()
    {
        var pumped = false;
        ObjectId bears = default;
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Forest"), Cards.Named("Giant Growth")), Lands("B"), new InspectingAgent(new GreedyAgent(attacks: false), state =>
        {
            if (state.Object(bears) is { } obj && Characteristics.Of(state, obj).Power == 5)
                pumped = true;
        }));
        bears = Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.True(pumped);
        Assert.Equal(2, Characteristics.Of(game.State, game.State.Object(bears)).Power);
        Assert.Empty(game.Skips);
    }

    [Fact]
    public void WrathOfGodDestroysEveryCreature()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Plains"), Cards.Basic("Plains"), Cards.Basic("Plains"), Cards.Basic("Plains"), Cards.Named("Wrath of God")), Lands("B"), new GreedyAgent(attacks: false));
        var mine = Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 0);
        var theirs = Put(game, Cards.Vanilla("Hill Giant", "{3}{R}", 3, 3, "Giant"), 1);
        foreach (var land in game.P0().Hand.Where(id => game.State.Object(id).Card.IsLand).Take(3).ToList())
            game.MoveToZone(land, Zone.Battlefield, new PlayerId(0));

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(mine).Zone);
        Assert.Equal(Zone.Graveyard, game.State.Object(theirs).Zone);
        Assert.Contains(game.Log, x => x is Destroyed);
    }

    [Fact]
    public void RaiseTheAlarmMakesTwoSoldierTokens()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Plains"), Cards.Basic("Plains"), Cards.Named("Raise the Alarm")), Lands("B"), new GreedyAgent(attacks: false));
        game.MoveToZone(game.P0().Hand[0], Zone.Battlefield, new PlayerId(0));

        game.PlayTurn(new PlayerId(0));

        var tokens = game.BattlefieldOf(0).Where(x => x.IsToken).ToList();
        Assert.Equal(2, tokens.Count);
        Assert.All(tokens, x => Assert.Equal(1, Characteristics.Of(game.State, x).Power));
        Assert.All(tokens, x => Assert.Contains("Soldier", x.Card.Subtypes));
    }

    [Fact]
    public void ElvishVisionaryDrawsACardWhenItEnters()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Forest"), Cards.Basic("Forest"), Cards.Named("Elvish Visionary")), Lands("B"), new GreedyAgent(attacks: false));
        game.MoveToZone(game.P0().Hand[0], Zone.Battlefield, new PlayerId(0));
        var before = game.P0().Hand.Count;

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Battlefield, game.Find("Elvish Visionary").Zone);
        // Drew for the turn? No: first turn skips the draw. Played a land (-1), cast the elf (-1), drew from its trigger (+1).
        Assert.Equal(before - 1, game.P0().Hand.Count);
        Assert.Contains(game.Log, x => x is Triggered);
    }

    [Fact]
    public void GloriousAnthemBuffsOnlyYourCreatures()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Put(game, Cards.Named("Glorious Anthem"), 0);
        var mine = Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 0);
        var theirs = Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 1);

        Assert.Equal(3, Characteristics.Of(game.State, game.State.Object(mine)).Power);
        Assert.Equal(2, Characteristics.Of(game.State, game.State.Object(theirs)).Power);
    }

    [Fact]
    public void ProdigalSorcererPingsWithItsTapAbility()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        var tim = Put(game, Cards.Named("Prodigal Sorcerer"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(19, game.P1().Life);
        Assert.True(game.State.Object(tim).Tapped);
        Assert.Contains(game.Log, x => x is AbilityActivated);
    }

    [Fact]
    public void TerrorCantTargetBlackOrArtifactCreatures()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Basic("Swamp"), Cards.Basic("Swamp"), Cards.Named("Terror")), Lands("B"), new PassiveAgent(), new PassiveAgent());
        game.MoveToZone(game.P0().Hand[0], Zone.Battlefield, new PlayerId(0));
        game.MoveToZone(game.P0().Hand[0], Zone.Battlefield, new PlayerId(0));
        var zombie = Put(game, Cards.Vanilla("Scathe Zombies", "{2}{B}", 2, 2, "Zombie"), 1);
        var bears = Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 1);
        var terror = game.Find("Terror");

        var slot = TargetSlots.Of(terror.Card.SpellEffects).Single();
        var candidates = TargetSlots.Candidates(slot, game, new PlayerId(0), terror.Id);

        Assert.Contains(TargetRef.Of(bears), candidates);
        Assert.DoesNotContain(TargetRef.Of(zombie), candidates);
    }

    [Fact]
    public void SoulWardenGainsLifeWhenAnyCreatureEnters()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Put(game, Cards.Named("Soul Warden"), 0);
        Put(game, Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear"), 1);

        game.CheckStateAndTriggers();
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(21, game.P0().Life);
    }

    [Fact]
    public void CounterspellCountersTheSpellOnTheStack()
    {
        var game = GameExtensions.Started(
            Hand("A", Cards.Basic("Mountain"), Cards.Named("Lightning Bolt")),
            Hand("B", Cards.Named("Counterspell")),
            new GreedyAgent(attacks: false),
            new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Island"), 1);
        Put(game, Cards.Basic("Island"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(20, game.P1().Life);
        Assert.Contains(game.Log, x => x is Countered);
        Assert.Equal(Zone.Graveyard, game.Find("Lightning Bolt").Zone);
        Assert.Equal(Zone.Graveyard, game.Find("Counterspell").Zone);
    }

    [Fact]
    public void FixtureCardsAreMostlyExecutable()
    {
        var report = new InterpretReport();
        var path = Path.Combine(AppContext.BaseDirectory, "Cards", "cards.jsonl");
        var cards = File.ReadLines(path).Select(IrJson.Deserialize<CardRules>).ToList();
        foreach (var card in cards)
            report.Add(card);

        var bolt = cards.Single(x => x.Name == "Lightning Bolt");
        Assert.Empty(bolt.Lines.SelectMany(x => x.Abilities).SelectMany(InterpreterSupport.Unsupported));
        Assert.True(report.LineFraction > 0.6, report.ToText());
    }
}

/// <summary>Wraps an agent and lets a test look at the state at every decision.</summary>
public sealed class InspectingAgent(IDecisionMaker inner, Action<GameState> inspect) : IDecisionMaker
{
    public DecisionResponse Decide(GameState state, Decision decision)
    {
        inspect(state);
        return inner.Decide(state, decision);
    }
}
