namespace MTGRulesIR.Tests;

/// <summary>The thirteenth round's shapes: tokens with a keyword ability, cards in all graveyards, counters equal to an amount, energy costs, either-or predicates, colors set, life compared, haunt, and permanents that entered this turn.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class RoundThirteenTests(GrammarFixture fixture)
{
    [Fact]
    public void ATokenWithProtectionKeepsTheKeyword()
    {
        var ability = fixture.LowerAbility("when this creature enters, target opponent creates two 3/3 green centaur creature tokens with protection from black.");

        var token = Assert.IsType<TokenSpec>(Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<CreateTokens>()).Token);
        var keyword = Assert.Single(token.Keywords);
        Assert.Equal("Protection", keyword.Keyword);
        Assert.Null(token.Abilities);
    }

    [Fact]
    public void CardsInGraveyardsAreEveryPlayersGraveyard()
    {
        var statics = Assert.IsType<StaticAbility>(fixture.LowerAbility("cards in graveyards can't be the targets of spells or abilities.", "Enchantment"));

        var cards = Assert.IsType<ObjectSelector>(Assert.IsType<CantBeTargeted>(statics.Effect).Objects);
        Assert.Equal("Graveyard", cards.Filter.Zone.Zone);
        Assert.Equal(PlayerKind.EachPlayer, Assert.IsType<PlayerSelector>(cards.Filter.Zone.Owner).Kind);
    }

    [Fact]
    public void CountersEqualToDevotionAreACountedSpec()
    {
        var ability = fixture.LowerAbility("when this creature enters, put a number of +1/+1 counters on it equal to your devotion to green.");

        var put = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<PutCounters>());
        Assert.Equal("+1/+1", put.Counters.Kind);
        Assert.IsType<Devotion>(put.Counters.Count);
    }

    [Fact]
    public void PayingEnergyIsEnergySymbols()
    {
        var ability = Assert.IsType<ActivatedAbility>(fixture.LowerAbility("{t}, pay eight {e}: this creature deals 6 damage to target player or planeswalker."));

        var energy = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<PayMana>(), x => x.Symbols.Contains("{e}"));
        Assert.Equal(8, energy.Symbols.Count);
        Assert.All(energy.Symbols, x => Assert.Equal("{e}", x));
    }

    [Fact]
    public void SacrificeOrDiscardIsAChoice()
    {
        var ability = fixture.LowerAbility("when this creature enters, you may sacrifice an artifact or discard a card. if you do, draw a card.");

        var choice = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<ChoiceOf>());
        Assert.IsType<Sacrifice>(choice.Options[0]);
        Assert.IsType<Discard>(choice.Options[1]);
    }

    [Fact]
    public void AllCreaturesAreBlackSetsTheColor()
    {
        var statics = Assert.IsType<StaticAbility>(fixture.LowerAbility("all creatures are black.", "Enchantment"));

        var becomes = Assert.IsType<BecomesAspect>(statics.Effect);
        Assert.Equal([new Term("Black", "ManaColor")], becomes.Aspects);
        Assert.False(becomes.InAddition);
    }

    [Fact]
    public void MoreLifeThanAnOpponentComparesLifeTotals()
    {
        var rules = fixture.Lower("you gain 10 life. then if you have more life than an opponent, create a 5/5 white giant warrior creature token.", "Sorcery");

        var compare = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(rules.Lines[0].Abilities[^1]).OfType<CompareCount>());
        Assert.Equal(Comparison.Greater, compare.Comparison);
        Assert.IsType<LifeTotal>(compare.Count);
        Assert.IsType<LifeTotal>(compare.Threshold);
    }

    [Fact]
    public void HauntTriggersOnEnteringOrTheHauntedCreatureDying()
    {
        var ability = Assert.IsType<TriggeredAbility>(fixture.LowerAbility("when this creature enters or the creature it haunts dies, destroy target creature."));

        var either = Assert.IsType<EitherEvent>(ability.Trigger);
        Assert.Equal(2, either.Events.Count);
        Assert.Contains(either.Events, x => x is ObjectEvent { Subject: ObjectSelector { Filter.With: [Related { Relation: Relation.HauntedBy }] } });
    }

    [Fact]
    public void ThatEnteredThisTurnIsAHistoryPredicate()
    {
        var rules = fixture.Lower("destroy target nonblack creature that entered this turn.", "Instant");

        var target = Assert.IsType<ObjectSelector>(Assert.IsType<Destroy>(Assert.IsType<SpellAbility>(Assert.Single(rules.Lines[0].Abilities)).Effect).Objects);
        var happened = Assert.IsType<Happened>(Assert.IsType<Satisfying>(Assert.Single(target.Filter.With)).Condition);
        Assert.Equal(["Entered"], Assert.IsType<ObjectEvent>(happened.Event).Events);
    }

    [Fact]
    public void DoublingCountersIsAReplacement()
    {
        var ability = fixture.LowerAbility("if one or more +1/+1 counters would be put on a creature you control, twice that many +1/+1 counters are put on it instead.");

        var replacement = Assert.IsType<Replacement>(Assert.IsType<ReplacementAbility>(ability).Effect);
        Assert.IsType<PutCounters>(Assert.IsType<WouldHappen>(replacement.Would).What);
        var instead = Assert.IsType<PutCounters>(Assert.IsType<Instead>(replacement.Instead).Effect);
        Assert.Equal(new Product(new Special("ThatMany"), new Fixed(2)), instead.Counters.Count);
    }
}
