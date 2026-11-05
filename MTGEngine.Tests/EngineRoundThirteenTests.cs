namespace MTGEngine.Tests;

/// <summary>The thirteenth engine round: typecycling, rebound, playing from the top of the library or a card's own graveyard, redirected damage, tapping or untapping, and amounts of life gained or damage prevented.</summary>
public sealed class EngineRoundThirteenTests
{
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

    static readonly PlayerSelector You = new(PlayerKind.You);

    [Fact]
    public void SwampcyclingFindsASwamp()
    {
        var game = GameExtensions.Started(Lands("A", "Swamp"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        for (var i = 0; i < 2; i++)
            Put(game, Cards.Basic("Swamp"), 0);
        var abomination = Put(game, Cards.Named("Twisted Abomination"), 0, Zone.Hand);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.State.Object(abomination).Zone);
        Assert.Contains(game.Log, x => x is ZoneMoved { From: Zone.Library, To: Zone.Hand });
    }

    [Fact]
    public void AReboundSpellIsCastAgainAtTheNextUpkeep()
    {
        var rules = new CardRules("Bounce Back", "Instant",
        [
            new LineRules(0, "rebound", [new KeywordLine([new KeywordInstance("Rebound")])], []),
            new LineRules(1, "you gain 3 life.", [new SpellAbility(new GainLife(You, new Fixed(3)))], []),
        ], new CardInfo("{W}"));
        var game = GameExtensions.Started(new Deck("A", [CardDefinition.From(rules), .. Enumerable.Repeat(Cards.Basic("Plains"), 20)]), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());

        game.PlayTurn(new PlayerId(0));
        var card = game.Find("Bounce Back");
        Assert.Equal(Zone.Exile, card.Zone);
        Assert.Equal(23, game.P0().Life);

        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Graveyard, game.Find("Bounce Back").Zone);
        Assert.Equal(26, game.P0().Life);
    }

    [Fact]
    public void StaggershockReboundsAtTheNextUpkeep()
    {
        var game = GameExtensions.Started(new Deck("A", [Cards.Named("Staggershock"), .. Enumerable.Repeat(Cards.Basic("Mountain"), 20)]), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        for (var i = 0; i < 2; i++)
            Put(game, Cards.Basic("Mountain"), 0);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Exile, game.Find("Staggershock").Zone);
        game.PlayTurn(new PlayerId(1));
        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.Find("Staggershock").Zone);
        Assert.Equal(16, game.P1().Life);
    }

    [Fact]
    public void RiftBoltIsSuspendedAndCastAtTheNextUpkeep()
    {
        var game = GameExtensions.Started(Lands("A", "Mountain"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Put(game, Cards.Basic("Mountain"), 0);
        var bolt = Put(game, Cards.Named("Rift Bolt"), 0, Zone.Hand);
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));
        var suspend = Assert.Single(game.LegalActions(new PlayerId(0)), x => x is CastSpell { Alternative: Game.SuspendCast });

        game.Take(suspend, new PlayerId(0));
        Assert.Equal(Zone.Exile, game.State.Object(bolt).Zone);
        Assert.Equal(1, game.State.Object(bolt).CountersOf("time"));

        var next = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        var exiled = Put(next, Cards.Named("Rift Bolt"), 0, Zone.Exile);
        next.Emit(new CountersChanged(exiled, "time", 1));
        next.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Graveyard, next.State.Object(exiled).Zone);
        Assert.Equal(17, next.P1().Life);
    }

    [Fact]
    public void FieryTemperIsCastForItsMadnessCostWhenDiscarded()
    {
        var game = GameExtensions.Started(Lands("A", "Mountain"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        Put(game, Cards.Basic("Mountain"), 0);
        var temper = Put(game, Cards.Named("Fiery Temper"), 0, Zone.Hand);

        game.Discard(new PlayerId(0), temper);
        Assert.Equal(Zone.Stack, game.State.Object(temper).Zone);
        game.PlayTurn(new PlayerId(1));

        Assert.Equal(Zone.Graveyard, game.State.Object(temper).Zone);
        Assert.Equal(17, game.P1().Life);
    }

    [Fact]
    public void CourserLetsYouPlayTheTopLand()
    {
        var game = GameExtensions.Started(Lands("A", "Forest"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Put(game, Cards.Named("Courser of Kruphix"), 0);
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));

        var top = game.P0().Library[0];
        Assert.Contains(game.LegalActions(new PlayerId(0)), x => x is PlayLand land && land.Card == top);
    }

    [Fact]
    public void GravecrawlerIsCastFromTheGraveyardWithAZombie()
    {
        var game = GameExtensions.Started(Lands("A", "Swamp"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        Put(game, Cards.Basic("Swamp"), 0);
        var crawler = Put(game, Cards.Named("Gravecrawler"), 0, Zone.Graveyard);
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));
        Assert.DoesNotContain(game.LegalActions(new PlayerId(0)), x => x is CastSpell cast && cast.Card == crawler);

        Put(game, Cards.Vanilla("Zombie", "{1}{B}", 2, 2, "Zombie"), 0);
        Assert.Contains(game.LegalActions(new PlayerId(0)), x => x is CastSpell cast && cast.Card == crawler);
    }

    [Fact]
    public void PersonalIncarnationRedirectsDamageToItsOwner()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var incarnation = Put(game, Cards.Named("Personal Incarnation"), 0);
        var redirect = Cards.Named("Personal Incarnation").Abilities.OfType<ActivatedAbility>().Single().Effect;

        Interpreter.Execute(redirect, Context(game, incarnation));
        game.DealDamage(null, TargetRef.Of(incarnation), 3, combat: false);

        Assert.Equal(2, game.State.Object(incarnation).Damage);
        Assert.Equal(19, game.P0().Life);
    }

    [Fact]
    public void TapOrUntapAsksWhich()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var land = Put(game, Cards.Basic("Plains"), 1);

        Interpreter.Execute(new TapOrUntap(new ObjectSelector(Quantifier.All, new ObjectFilter(Kinds: [new Term("Land", "CardType")]))), Context(game));

        // The passive agent picks the first option, tapping.
        Assert.True(game.State.Object(land).Tapped);
    }

    [Fact]
    public void PhyrexianHydraTurnsDamageIntoCounters()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var hydra = Put(game, Cards.Named("Phyrexian Hydra"), 0);

        game.DealDamage(null, TargetRef.Of(hydra), 3, combat: false);

        Assert.Equal(0, game.State.Object(hydra).Damage);
        Assert.Equal(3, game.State.Object(hydra).CountersOf("-1/-1"));
    }

    [Fact]
    public void VoraciousWurmCountsLifeGainedThisTurn()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        game.ChangeLife(new PlayerId(0), 3, "test");

        var wurm = Put(game, Cards.Named("Voracious Wurm"), 0);

        Assert.Equal(3, game.State.Object(wurm).CountersOf("+1/+1"));
    }
}

/// <summary>A permanent's line that states a continuous effect with no trigger or cost is a static ability.</summary>
public sealed class PermanentEffectLineTests
{
    static ObjectId Put(Game game, CardDefinition card, int player)
    {
        var id = new ObjectId(game.State.NextObjectId);
        var owner = new PlayerId(player);
        game.Emit(new ObjectCreated(new GameObject(id, card, owner, owner, Zone.Hand)));
        game.MoveToZone(id, Zone.Battlefield, owner);
        game.Emit(new SummoningSicknessCleared(owner));
        return id;
    }

    [Fact]
    public void GloryOfWarfarePumpsDuringYourTurnOnly()
    {
        var game = GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Plains"), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new PassiveAgent(), new PassiveAgent());
        Put(game, Cards.Named("Glory of Warfare"), 0);
        var bears = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);

        game.Emit(new TurnBegan(1, new PlayerId(0)));
        Assert.Equal(4, Characteristics.Of(game.State, game.State.Object(bears)).Power);
        game.Emit(new TurnBegan(2, new PlayerId(1)));
        Assert.Equal(2, Characteristics.Of(game.State, game.State.Object(bears)).Power);
    }

    [Fact]
    public void AkoumFirebirdAttacksEachCombat()
    {
        var game = GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Mountain"), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new GreedyAgent(attacks: false), new PassiveAgent());
        var firebird = Put(game, Cards.Named("Akoum Firebird"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Contains(game.Log, x => x is AttackersDeclared declared && declared.Attackers.Contains(firebird));
    }
}

/// <summary>Damage modifiers: "it deals double that damage instead", "that much damage plus 2", "minus 1".</summary>
public sealed class DamageModifierTests
{
    static ObjectId Put(Game game, CardDefinition card, int player)
    {
        var id = new ObjectId(game.State.NextObjectId);
        var owner = new PlayerId(player);
        game.Emit(new ObjectCreated(new GameObject(id, card, owner, owner, Zone.Hand)));
        game.MoveToZone(id, Zone.Battlefield, owner);
        game.Emit(new SummoningSicknessCleared(owner));
        return id;
    }

    static Game Started() =>
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Mountain"), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new PassiveAgent(), new PassiveAgent());

    [Fact]
    public void GratuitousViolenceDoublesYourCreaturesDamage()
    {
        var game = Started();
        Put(game, Cards.Named("Gratuitous Violence"), 0);
        var mine = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);
        var theirs = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 1);

        game.DealDamage(mine, TargetRef.Of(new PlayerId(1)), 2, combat: true);
        game.DealDamage(theirs, TargetRef.Of(new PlayerId(0)), 2, combat: true);

        Assert.Equal(16, game.P1().Life);
        Assert.Equal(18, game.P0().Life);
    }

    [Fact]
    public void TorbranAddsTwoToRedSourcesAgainstOpponents()
    {
        var game = Started();
        var torbran = Put(game, Cards.Named("Torbran, Thane of Red Fell"), 0);

        game.DealDamage(torbran, TargetRef.Of(new PlayerId(1)), 2, combat: true);
        game.DealDamage(torbran, TargetRef.Of(new PlayerId(0)), 2, combat: false);

        Assert.Equal(16, game.P1().Life);
        Assert.Equal(18, game.P0().Life);
    }

    [Fact]
    public void BenevolentUnicornReducesSpellDamage()
    {
        var game = Started();
        Put(game, Cards.Named("Benevolent Unicorn"), 1);
        var bolt = new ObjectId(game.State.NextObjectId);
        game.Emit(new ObjectCreated(new GameObject(bolt, Cards.Named("Lightning Bolt"), new PlayerId(0), new PlayerId(0), Zone.Stack)));
        var creature = Put(game, Cards.Vanilla("Bears", "{1}{G}", 2, 2), 0);

        game.DealDamage(bolt, TargetRef.Of(new PlayerId(1)), 3, combat: false);
        game.DealDamage(creature, TargetRef.Of(new PlayerId(1)), 2, combat: true);

        Assert.Equal(20 - 2 - 2, game.P1().Life);
    }
}

public sealed class ReflexiveTriggerTests
{
    [Fact]
    public void BreechesFlipResultsArePartOfItsTrigger()
    {
        // "When you win the flip" was a state trigger on the last flip, so it fired again after every resolution.
        var card = Cards.Named("Breeches, the Blastmaker");

        Assert.DoesNotContain(card.Abilities, x => x is TriggeredAbility { Trigger: ConditionEvent });
        Assert.Contains(card.Abilities, x => x is TriggeredAbility { Trigger: SpellCastEvent });
    }
}

/// <summary>Restricted mana in the pool, and mana abilities with costs besides {T}.</summary>
public sealed class RestrictedManaTests
{
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

    static Game Started() =>
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Mountain"), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new PassiveAgent(), new PassiveAgent());

    [Fact]
    public void GeosurgeManaPaysOnlyForArtifactsAndCreatures()
    {
        var game = Started();
        var surge = Put(game, Cards.Named("Geosurge"), 0, Zone.Hand);
        var ctx = new EffectContext(game, new PlayerId(0), surge, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));
        foreach (var effect in Cards.Named("Geosurge").SpellEffects)
            ctx = Interpreter.Execute(effect, ctx);

        Assert.Equal(0, game.P0().Pool.Total);
        Assert.Equal(7, game.P0().RestrictedList.Sum(x => x.Amount));
        var giantId = Put(game, Cards.Vanilla("Giant", "{6}{R}", 7, 7), 0, Zone.Hand);
        var boltId = Put(game, Cards.Named("Lightning Bolt"), 0, Zone.Hand);
        var creature = game.State.Object(giantId);
        var bolt = game.State.Object(boltId);
        Assert.True(ManaPayment.CanPay(game.State, new PlayerId(0), creature.Card.ManaCost, purpose: new ManaPayment.Purpose(Spell: creature)));
        Assert.False(ManaPayment.CanPay(game.State, new PlayerId(0), bolt.Card.ManaCost, purpose: new ManaPayment.Purpose(Spell: bolt)));
    }

    [Fact]
    public void AgentOfStromgaldTurnsRedManaBlack()
    {
        var game = Started();
        var agent = Put(game, Cards.Named("Agent of Stromgald"), 0);
        Put(game, Cards.Basic("Mountain"), 0);
        game.Emit(new TurnBegan(1, new PlayerId(0)));
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));

        var activate = Assert.Single(game.LegalActions(new PlayerId(0)), x => x is ActivateAbility a && a.Source == agent);
        game.Take(activate, new PlayerId(0));

        Assert.Equal(1, game.P0().Pool.Of(Color.Black));
        Assert.Empty(game.State.Stack);
    }
}
