namespace MTGEngine.Tests;

/// <summary>The fourth engine round: looking and choosing among what was found, playing cards from exile, characteristic-defining power, combat rules, uncounterable spells, heroic, token copies, hand-size conditions, draw triggers, activation timing.</summary>
public sealed class EngineRoundFourTests
{
    static Deck Hand(string player, params CardDefinition[] cards) =>
        new(player, [.. cards, .. Enumerable.Repeat(Cards.Basic("Plains"), 20)]);

    static Deck Lands(string player, string land = "Plains") => Cards.DeckOf(player, (Cards.Basic(land), 20));

    static ObjectId Put(Game game, CardDefinition card, int player, Zone zone = Zone.Battlefield)
    {
        var id = new ObjectId(game.State.NextObjectId);
        var owner = new PlayerId(player);
        game.Emit(new ObjectCreated(new GameObject(id, card, owner, owner, Zone.Hand)));
        if (zone != Zone.Hand)
            game.MoveToZone(id, zone, owner);
        game.Emit(new SummoningSicknessCleared(owner));
        return id;
    }

    static EffectContext Context(Game game, ObjectId? source = null) =>
        new(game, new PlayerId(0), source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));

    static readonly CardDefinition Bears = Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear");

    [Fact]
    public void ImpulsePutsOneIntoHandAndTheRestOnTheBottom()
    {
        var library = Enumerable.Range(0, 10).Select(i => Cards.Vanilla($"Card {i}", "{1}", 1, 1)).ToArray();
        var game = GameExtensions.Started(new Deck("A", [Cards.Basic("Island"), Cards.Named("Impulse"), .. Enumerable.Repeat(Cards.Basic("Plains"), 5), .. library]), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Island"), 0);
        var top = game.P0().Library.Take(4).ToList();

        game.PlayTurn(new PlayerId(0));

        var inHand = game.P0().Hand.Intersect(top).ToList();
        Assert.Single(inHand);
        Assert.Equal(top.Except(inHand).OrderBy(x => x.Value), game.P0().Library.TakeLast(3).OrderBy(x => x.Value));
    }

    [Fact]
    public void AbbotLetsYouPlayTheExiledCardThisTurn()
    {
        var game = GameExtensions.Started(new Deck("A", [Cards.Named("Abbot of Keral Keep"), Cards.Basic("Mountain"), .. Enumerable.Repeat(Cards.Basic("Plains"), 5), Cards.Basic("Mountain"), .. Enumerable.Repeat(Cards.Basic("Plains"), 10)]), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent(), handSize: 2);
        Put(game, Cards.Basic("Mountain"), 0);

        game.PlayTurn(new PlayerId(0));

        // The land in hand pays for the abbot; the exiled card is a land, but the land drop is used, so it stays in exile, playable only this turn.
        var exiled = game.P0().Exile.Select(game.State.Object).Single();
        Assert.Contains(game.Log, x => x is PermissionGranted { Permission.Card: var card } && card == exiled.Id);
        Assert.DoesNotContain(game.LegalActions(new PlayerId(0)), x => x is PlayLand play && play.Card == exiled.Id);
    }

    [Fact]
    public void APermissionLetsYouCastFromExile()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Mountain"), 0);
        var bolt = Put(game, Cards.Named("Lightning Bolt"), 0, Zone.Exile);
        var ctx = Context(game) with { Last = [TargetRef.Of(bolt)] };

        Interpreter.Execute(new PlayCard(new PlayerSelector(PlayerKind.You), new Reference(null, "it"), Duration: new ThisTurn()), ctx);
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(17, game.P1().Life);
        Assert.Equal(Zone.Graveyard, game.State.Object(bolt).Zone);
    }

    [Fact]
    public void MarosPowerIsTheCardsInHand()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var maro = Put(game, Cards.Named("Maro"), 0);

        var traits = Characteristics.Of(game.State, game.State.Object(maro));

        Assert.Equal(game.P0().Hand.Count, traits.Power);
        Assert.Equal(game.P0().Hand.Count, traits.Toughness);
    }

    [Fact]
    public void BogRatsCantBeBlockedByWalls()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new GreedyAgent(blocks: true));
        Put(game, Cards.Named("Bog Rats"), 0);
        Put(game, Cards.Vanilla("Wall of Wood", "{G}", 0, 3, "Wall"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.DoesNotContain(game.Log, x => x is BlockersDeclared);
        Assert.Equal(19, game.P1().Life);
    }

    [Fact]
    public void AmrouKithkinCantBeBlockedByBigCreatures()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new GreedyAgent(blocks: true));
        Put(game, Cards.Named("Amrou Kithkin"), 0);
        Put(game, Cards.Vanilla("Hill Giant", "{3}{R}", 3, 3, "Giant"), 1);
        Put(game, Cards.Vanilla("Raging Goblin", "{R}", 1, 1, "Goblin"), 1);

        game.PlayTurn(new PlayerId(0));

        var blocks = Assert.Single(game.Log.OfType<BlockersDeclared>()).Blocks;
        Assert.Equal("Raging Goblin", game.State.Object(Assert.Single(blocks).Key).Card.Name);
    }

    [Fact]
    public void GorillaBerserkersNeedThreeBlockers()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new GreedyAgent(blocks: true));
        Put(game, Cards.Named("Gorilla Berserkers"), 0);
        Put(game, Bears, 1);
        Put(game, Bears, 1);

        game.PlayTurn(new PlayerId(0));

        Assert.DoesNotContain(game.Log, x => x is BlockersDeclared);
        Assert.Equal(18, game.P1().Life);
    }

    [Fact]
    public void AbruptDecayCantBeCountered()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var decay = Put(game, Cards.Named("Abrupt Decay"), 0, Zone.Hand);
        game.MoveToZone(decay, Zone.Stack, new PlayerId(0));
        game.Emit(new SpellCast(new StackItem(game.State.NextStackId, new PlayerId(0), decay, StackKind.Spell, [], [], Spell: decay)));

        game.CounterSpell(decay);

        Assert.Equal(Zone.Stack, game.State.Object(decay).Zone);
        Assert.DoesNotContain(game.Log, x => x is Countered);
    }

    [Fact]
    public void HeroicTriggersOnASpellThatTargetsIt()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Named("Giant Growth")), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Forest"), 0);
        var leader = Put(game, Cards.Named("Phalanx Leader"), 0);
        var bears = Put(game, Bears, 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(leader).CountersOf("+1/+1"));
        Assert.Equal(1, game.State.Object(bears).CountersOf("+1/+1"));
    }

    [Fact]
    public void CacklingCounterpartCopiesACreature()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        var serra = Put(game, Cards.Named("Serra Angel"), 0);
        var ctx = Context(game);
        var copy = Cards.Named("Cackling Counterpart").SpellEffects.Single();
        var slot = Assert.Single(TargetSlots.Of([copy]));
        ctx.TargetsBySelector[slot.Selector] = [TargetRef.Of(serra)];

        Interpreter.Execute(copy, ctx);

        var token = Assert.Single(game.State.Battlefield, x => x.IsToken);
        Assert.Equal("Serra Angel", token.Card.Name);
        Assert.True(Characteristics.Of(game.State, token).HasKeyword("Flying"));
    }

    [Fact]
    public void AkkiUnderlingGrowsWithAFullHand()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var akki = Put(game, Cards.Named("Akki Underling"), 0);

        Assert.Equal(game.P0().Hand.Count >= 7 ? 4 : 2, Characteristics.Of(game.State, game.State.Object(akki)).Power);
        game.Draw(new PlayerId(0), 1);
        Assert.Equal(4, Characteristics.Of(game.State, game.State.Object(akki)).Power);
    }

    [Fact]
    public void ChasmSkulkerGrowsAsYouDraw()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var skulker = Put(game, Cards.Named("Chasm Skulker"), 0);

        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(1, game.State.Object(skulker).CountersOf("+1/+1"));
    }

    [Fact]
    public void ActivateOnlyDuringYourUpkeep()
    {
        var ability = new ActivatedAbility([new PayMana(["{0}"])], new GainLife(new PlayerSelector(PlayerKind.You), new Fixed(1)));
        var card = Cards.Vanilla("Upkeeper", "{1}", 1, 1) with
        {
            Abilities = [ability, new ActivationLimits([new Timing("During", new PhaseRef("Upkeep", Whose: new PlayerSelector(PlayerKind.You)))])],
        };
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Put(game, card, 0);

        Assert.DoesNotContain(game.LegalActions(new PlayerId(0)), x => x is ActivateAbility);
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Upkeep, new PlayerId(0)));
        Assert.Contains(game.LegalActions(new PlayerId(0)), x => x is ActivateAbility);
    }
}
