using MTGEngine.Agents;

namespace MTGEngine.Tests;

/// <summary>The twenty-eighth engine round: two-faced cards. Transforming, flipping, day and night with daybound and nightbound, and casting from the graveyard transformed with disturb.</summary>
public sealed class EngineRoundTwentyEightTests
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

    static Game Started(IDecisionMaker first = null, IDecisionMaker second = null, int handSize = 0) =>
        GameExtensions.Started(Cards.DeckOf("A", (Cards.Basic("Mountain"), 20)), Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), first ?? new PassiveAgent(), second ?? new PassiveAgent(), handSize);

    static EffectContext Context(Game game, ObjectId? source = null) =>
        new(game, new PlayerId(0), source, new Dictionary<IrNode, ImmutableList<TargetRef>>(ReferenceEqualityComparer.Instance));

    [Fact]
    public void ATwoFacedCardCarriesItsBackFace()
    {
        var delver = Cards.Named("Delver of Secrets // Insectile Aberration");

        Assert.NotNull(delver.Back);
        Assert.Equal("Insectile Aberration", delver.Back.Name);
        Assert.Equal(3, delver.Back.Power);
        Assert.Equal(delver.ManaCost, delver.Back.ManaCost);
        Assert.Contains(Color.Blue, delver.Back.Colors);
    }

    [Fact]
    public void DelverTransformsWhenAnInstantIsOnTop()
    {
        var deck = new Deck("A", [Cards.Named("Lightning Bolt"), .. Enumerable.Repeat(Cards.Basic("Mountain"), 20)]);
        var game = GameExtensions.Started(deck, Cards.DeckOf("B", (Cards.Basic("Plains"), 20)), new GreedyAgent(attacks: false), new PassiveAgent(), handSize: 0);
        var delver = Put(game, Cards.Named("Delver of Secrets // Insectile Aberration"), 0);

        game.PlayTurn(new PlayerId(0));

        var transformed = game.State.Object(delver);
        Assert.True(transformed.Transformed);
        Assert.Equal("Insectile Aberration", transformed.Card.Name);
        var traits = Characteristics.Of(game.State, transformed);
        Assert.Equal(3, traits.Power);
        Assert.True(traits.HasKeyword("Flying"));
    }

    [Fact]
    public void ATransformedPermanentLeavesFrontFaceUp()
    {
        var game = Started();
        var delver = Put(game, Cards.Named("Delver of Secrets // Insectile Aberration"), 0);

        Interpreter.Execute(new Transform(new Self()), Context(game, delver));
        Assert.Equal("Insectile Aberration", game.State.Object(delver).Card.Name);

        Interpreter.Execute(new Transform(new Self()), Context(game, delver));
        Assert.Equal("Delver of Secrets // Insectile Aberration", game.State.Object(delver).Card.Name);
        Assert.False(game.State.Object(delver).Transformed);

        Interpreter.Execute(new Transform(new Self()), Context(game, delver));
        game.MoveToZone(delver, Zone.Hand);
        Assert.False(game.State.Object(delver).Transformed);
        Assert.Equal("Delver of Secrets // Insectile Aberration", game.State.Object(delver).Card.Name);
    }

    [Fact]
    public void AkkiLavarunnerFlipsOnceWhenItDamagesAnOpponent()
    {
        var game = Started();
        var akki = Put(game, Cards.Named("Akki Lavarunner // Tok-Tok, Volcano Born"), 0);

        game.DealDamage(akki, TargetRef.Of(new PlayerId(1)), 1, combat: true);
        game.ResolveStack();

        var flipped = game.State.Object(akki);
        Assert.Equal("Tok-Tok, Volcano Born", flipped.Card.Name);
        Assert.Contains("Legendary", flipped.Card.Supertypes);
        Assert.Equal(2, Characteristics.Of(game.State, flipped).Power);

        // Flipping a flipped card does nothing.
        Assert.False(game.Transform(akki, toBack: true));
        Assert.Equal("Tok-Tok, Volcano Born", game.State.Object(akki).Card.Name);
    }

    [Fact]
    public void HermitTransformsWhenNoSpellsWereCastLastTurn()
    {
        var game = Started();
        var hermit = Put(game, Cards.Named("Hermit of the Natterknolls // Lone Wolf of the Natterknolls"), 0);

        game.PlayTurn(new PlayerId(0));
        game.PlayTurn(new PlayerId(1));

        Assert.Equal("Lone Wolf of the Natterknolls", game.State.Object(hermit).Card.Name);
    }

    [Fact]
    public void ADayboundPermanentStartsTheDayAndTransformsAtNight()
    {
        var game = Started();
        Assert.Null(game.State.DayNight);
        var ruffian = Put(game, Cards.Named("Tavern Ruffian // Tavern Smasher"), 0);
        Assert.Equal("Day", game.State.DayNight);

        Interpreter.Execute(new SetDayNight("Night"), Context(game));

        Assert.Equal("Night", game.State.DayNight);
        Assert.Equal("Tavern Smasher", game.State.Object(ruffian).Card.Name);
    }

    [Fact]
    public void NightFallsAfterATurnWithNoSpells()
    {
        var game = Started();
        var ruffian = Put(game, Cards.Named("Tavern Ruffian // Tavern Smasher"), 0);

        game.PlayTurn(new PlayerId(0));
        Assert.Equal("Day", game.State.DayNight);
        game.PlayTurn(new PlayerId(1));

        Assert.Equal("Night", game.State.DayNight);
        Assert.Equal("Tavern Smasher", game.State.Object(ruffian).Card.Name);
    }

    [Fact]
    public void BaithookAnglerIsCastFromTheGraveyardTransformedWithDisturb()
    {
        var game = Started(new GreedyAgent(attacks: false));
        Put(game, Cards.Basic("Island"), 0);
        Put(game, Cards.Basic("Island"), 0);
        var angler = Put(game, Cards.Named("Baithook Angler // Hook-Haunt Drifter"), 0, Zone.Graveyard);
        game.Emit(new StepBegan(Step.Main1, new PlayerId(0)));

        var disturb = Assert.Single(game.LegalActions(new PlayerId(0)), x => x is CastSpell { Alternative: Game.DisturbCast } cast && cast.Card == angler);
        game.Take(disturb, new PlayerId(0));
        game.ResolveStack();

        var drifter = game.State.Object(angler);
        Assert.Equal(Zone.Battlefield, drifter.Zone);
        Assert.Equal("Hook-Haunt Drifter", drifter.Card.Name);
        Assert.True(Characteristics.Of(game.State, drifter).HasKeyword("Flying"));

        // The back face exiles itself instead of going to a graveyard, and is front face up there.
        game.Destroy(angler);
        Assert.Equal(Zone.Exile, game.State.Object(angler).Zone);
        Assert.Equal("Baithook Angler // Hook-Haunt Drifter", game.State.Object(angler).Card.Name);
    }
}
