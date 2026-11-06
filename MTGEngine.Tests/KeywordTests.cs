namespace MTGEngine.Tests;

/// <summary>Keywords the engine carries out, on real cards where the fixture has them.</summary>
public sealed class KeywordTests
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

    static readonly CardDefinition Bears = Cards.Vanilla("Grizzly Bears", "{1}{G}", 2, 2, "Bear");

    static CardDefinition With(CardDefinition card, params KeywordInstance[] keywords) =>
        CardDefinition.From(new CardRules(card.Name, string.Join(",", card.Types), [new LineRules(0, "", [new KeywordLine(keywords)], [])],
            new CardInfo(card.ManaCost.ToString(), Subtypes: string.Join(",", card.Subtypes), Power: card.Power?.ToString(), Toughness: card.Toughness?.ToString())));

    [Fact]
    public void BonesplitterEquipsAndPumps()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new PassiveAgent());
        Put(game, Cards.Named("Bonesplitter"), 0);
        var bears = Put(game, Bears, 0);
        Put(game, Cards.Basic("Plains"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(bears, game.Find("Bonesplitter").AttachedTo);
        Assert.Equal(16, game.P1().Life);
    }

    [Fact]
    public void CyclingDiscardsToDraw()
    {
        var cycler = With(Cards.Vanilla("Cycler", "{5}", 5, 5), new KeywordInstance("Cycling", Cost: new PayMana(["{1}"])));
        var game = GameExtensions.Started(Hand("A", cycler), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Graveyard, game.Find("Cycler").Zone);
        Assert.Contains(game.Log, x => x is Discarded);
    }

    [Fact]
    public void ThinkTwiceFlashesBackFromTheGraveyardAndIsExiled()
    {
        var game = GameExtensions.Started(Lands("A", "Island"), Lands("B"), new GreedyAgent(attacks: false), new PassiveAgent());
        for (var i = 0; i < 3; i++)
            Put(game, Cards.Basic("Island"), 0);
        var twice = Put(game, Cards.Named("Think Twice"), 0, Zone.Graveyard);
        var hand = game.P0().Hand.Count;

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(Zone.Exile, game.State.Object(twice).Zone);
        Assert.True(game.P0().Hand.Count >= hand);
    }

    [Fact]
    public void WhiteKnightIsProtectedFromBlack()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var knight = Put(game, Cards.Named("White Knight"), 0);
        var black = Put(game, Cards.Vanilla("Black Knight", "{B}{B}", 2, 2, "Knight"), 1);
        var red = Put(game, Cards.Vanilla("Red Knight", "{R}{R}", 2, 2, "Knight"), 1);

        game.DealDamage(black, TargetRef.Of(knight), 2, combat: false);
        Assert.Equal(0, game.State.Object(knight).Damage);
        game.DealDamage(red, TargetRef.Of(knight), 1, combat: false);
        Assert.Equal(1, game.State.Object(knight).Damage);

        var doomBlade = Put(game, Cards.Named("Doom Blade"), 1, Zone.Hand);
        var slot = TargetSlots.Of(Cards.Named("Doom Blade").SpellEffects).Single();
        Assert.DoesNotContain(TargetRef.Of(knight), TargetSlots.Candidates(slot, game, new PlayerId(1), doomBlade));
    }

    [Fact]
    public void BogWraithWalksThroughSwamps()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new GreedyAgent(blocks: true));
        Put(game, Cards.Named("Bog Wraith"), 0);
        Put(game, Bears, 1);
        Put(game, Cards.Basic("Swamp"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.DoesNotContain(game.Log, x => x is BlockersDeclared { Blocks.Count: > 0 });
        Assert.Equal(17, game.P1().Life);
    }

    [Fact]
    public void KitchenFinksPersists()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var finks = Put(game, Cards.Named("Kitchen Finks"), 0);
        game.CheckStateAndTriggers();

        game.Destroy(finks);
        game.PlayTurn(new PlayerId(1));

        Assert.Equal(Zone.Battlefield, game.State.Object(finks).Zone);
        Assert.Equal(1, game.State.Object(finks).CountersOf("-1/-1"));
        game.Destroy(finks);
        game.PlayTurn(new PlayerId(0));
        Assert.Equal(Zone.Graveyard, game.State.Object(finks).Zone);
    }

    [Fact]
    public void NobleHierarchExaltsALoneAttacker()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new PassiveAgent());
        var hierarch = Put(game, Cards.Named("Noble Hierarch"), 0);
        game.Emit(new TurnBegan(1, new PlayerId(1)));
        Put(game, Bears, 0);

        game.PlayTurn(new PlayerId(0));

        // Both the hierarch and the bears can attack; exalted fires only if one attacks alone.
        var attackers = game.Log.OfType<AttackersDeclared>().Single().Attackers.Count;
        Assert.Equal(attackers == 1 ? 17 : 20 - 2 - 0, game.P1().Life);
    }

    [Fact]
    public void ProwessPumpsOnNoncreatureSpells()
    {
        var game = GameExtensions.Started(Hand("A", Cards.Named("Lightning Bolt")), Lands("B"), new GreedyAgent(attacks: true), new PassiveAgent());
        Put(game, Cards.Named("Monastery Swiftspear"), 0);
        Put(game, Cards.Basic("Mountain"), 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(20 - 3 - 2, game.P1().Life);
    }

    [Fact]
    public void SmugglersCopterIsCrewed()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new PassiveAgent());
        var copter = Put(game, Cards.Named("Smuggler's Copter"), 0);
        Put(game, Bears, 0);

        game.PlayTurn(new PlayerId(0));

        Assert.Contains(game.Log, x => x is AttackersDeclared declared && declared.Attackers.Contains(copter));
    }

    [Fact]
    public void BoggartRamGangWithersBlockers()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new GreedyAgent(attacks: true), new GreedyAgent(blocks: true));
        Put(game, Cards.Named("Boggart Ram-Gang"), 0);
        var wall = Put(game, Cards.Vanilla("Wall", "{2}", 0, 5, "Wall"), 1);

        game.PlayTurn(new PlayerId(0));

        Assert.Equal(3, game.State.Object(wall).CountersOf("-1/-1"));
        Assert.Equal(0, game.State.Object(wall).Damage);
    }

    [Fact]
    public void FrogmiteIsFreeWithFourArtifacts()
    {
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        for (var i = 0; i < 4; i++)
            Put(game, Cards.Named("Ornithopter"), 0);
        var frogmite = Put(game, Cards.Named("Frogmite"), 0, Zone.Hand);

        Assert.True(game.CostToCast(game.State.Object(frogmite), new PlayerId(0), Cards.Named("Frogmite").ManaCost, null).IsFree);
    }

    [Fact]
    public void ADoubleFacedCardDoesntHaveItsBackFacesKeywords()
    {
        // The database lists keywords of both faces; Delver of Secrets' front face doesn't fly.
        var rules = new CardRules("Delver of Secrets // Insectile Aberration", "Creature", [], new CardInfo("{U}", Subtypes: "Human,Wizard", Power: "1", Toughness: "1", Keywords: ["Flying", "Transform"]));

        Assert.DoesNotContain("Flying", CardDefinition.From(rules).Keywords);
    }

    [Fact]
    public void DevoidIsColorlessAndChangelingIsEveryType()
    {
        var devoid = With(Cards.Vanilla("Eldrazi", "{1}{C}{R}", 2, 2, "Eldrazi"), new KeywordInstance("Devoid"));
        var changeling = With(Cards.Vanilla("Shapeshifter", "{1}", 1, 1, "Shapeshifter"), new KeywordInstance("Changeling"));
        var game = GameExtensions.Started(Lands("A"), Lands("B"), new PassiveAgent(), new PassiveAgent());
        var eldrazi = Put(game, devoid, 0);
        var shifter = Put(game, changeling, 0);

        Assert.Empty(Characteristics.Of(game.State, game.State.Object(eldrazi)).Colors);
        Assert.True(Filters.Matches(game.State, new ObjectFilter(Kinds: [new Term("Goblin", "CreatureType")]), game.State.Object(shifter), new PlayerId(0), null));
    }
}
