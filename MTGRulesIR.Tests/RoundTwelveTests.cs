namespace MTGRulesIR.Tests;

/// <summary>Shapes the thirteenth engine round needed from the lowering: typecycling's type, "card" and "spell" as object kinds, "minus" in an adjusted amount.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class RoundTwelveTests(GrammarFixture fixture)
{
    [Fact]
    public void TypecyclingKeepsItsType()
    {
        var rules = fixture.Lower("swampcycling {2}", "Creature");

        var keyword = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(rules.Lines[0].Abilities[0]).OfType<KeywordInstance>());
        Assert.Equal("Typecycling", keyword.Keyword);
        Assert.Equal("swamp", keyword.Parameter);
    }

    [Fact]
    public void MinusLowersAsMinus()
    {
        var ability = fixture.LowerAbility("if a spell would deal damage to a permanent or player, it deals that much damage minus 1 to that permanent or player instead");

        var adjusted = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<Adjusted>());
        Assert.True(adjusted.Minus);
    }

    [Fact]
    public void WhenYouWinTheFlipIsPartOfTheAbilityThatFlips()
    {
        var rules = fixture.Lower("whenever you cast your second spell each turn, flip a coin. when you win the flip, you gain 2 life. when you lose the flip, you lose 2 life.", "Creature");

        var ability = Assert.IsType<TriggeredAbility>(Assert.Single(rules.Lines[0].Abilities));
        Assert.Equal(2, MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<Conditional>().Count(x => x.If is OutcomeCondition));
    }

    [Fact]
    public void ThatSpellIsASpell()
    {
        var ability = fixture.LowerAbility("whenever a player casts a spell, that player gains x life, where x is the number of cards in all graveyards with the same name as that spell");

        Assert.DoesNotContain(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<ObjectSelector>(), x => x.Filter?.Kinds?.Any(k => k.Vocabulary == "StackObjectKind") == true);
    }

    [Fact]
    public void TheExiledCardIsOneThisExiled()
    {
        var ability = fixture.LowerAbility("when this creature leaves the battlefield, return the exiled card to the battlefield under its owner's control.");

        var those = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<ObjectSelector>(), x => x.Quantifier == Quantifier.Those);
        Assert.Contains(those.Filter.With, x => x is Related { Relation: Relation.ExiledWith, To: Self });
    }

    [Fact]
    public void RevealedCardIsWhatGoesToHandNotThisCreature()
    {
        var ability = fixture.LowerAbility("when this creature enters, look at the top four cards of your library. you may reveal a human card from among them and put it into your hand. put the rest on the bottom of your library in a random order.");

        Assert.DoesNotContain(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<ChangeZone>(), x => x.Objects is Self);
        Assert.DoesNotContain(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<Related>(), x => x is { Relation: Relation.Among, To: Self });
    }

    [Fact]
    public void ItAfterLookingIsTheCardLookedAt()
    {
        var ability = fixture.LowerAbility("whenever this creature or another elemental you control enters, look at the top card of your library. if it's a land card, you may put it onto the battlefield tapped. if you don't put the card onto the battlefield, put it into your hand.");

        Assert.All(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<ChangeZone>(), x => Assert.IsType<ObjectSelector>(x.Objects));
    }

    [Fact]
    public void ItDoesntUntapIsWhatWasTapped()
    {
        var ability = fixture.LowerAbility("whenever this creature deals combat damage to a creature, tap that creature and it doesn't untap during its controller's next untap step.");

        Assert.IsNotType<Self>(Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<DoesntUntap>()).Objects);
    }

    [Fact]
    public void TargetCreatureBlocksThisCreature()
    {
        var ability = fixture.LowerAbility("when this creature enters, you may have target creature block it this turn if able.");

        Assert.IsType<Self>(Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<MustBlock>()).Attacker);
    }

    [Fact]
    public void ThreeOrMoreIsAtLeastThree()
    {
        var ability = fixture.LowerAbility("this creature gets +2/+2 as long as you control three or more artifacts.");

        var bound = Assert.IsType<Bound>(Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<ObjectSelector>(), x => x.Count is Bound).Count);
        Assert.Equal(Comparison.GreaterOrEqual, bound.Comparison);
    }

    [Fact]
    public void ItAfterChoosingIsWhatWasChosen()
    {
        var ability = fixture.LowerAbility("when this creature enters, an opponent chooses a permanent you control other than this creature and exiles it.");

        Assert.IsType<Reference>(Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<Exile>()).Objects);
    }

    [Fact]
    public void AGraveyardIsAnyPlayers()
    {
        var ability = fixture.LowerAbility("{1}, sacrifice this artifact: exile target card from a graveyard.");

        var target = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<ObjectSelector>(), x => x.Quantifier == Quantifier.Target);
        Assert.Equal(PlayerKind.EachPlayer, Assert.IsType<PlayerSelector>(target.Filter.Zone.Owner).Kind);
    }

    [Fact]
    public void ItsInTheConditionIsTheTarget()
    {
        var ability = fixture.LowerAbility("destroy target nonblack creature if its toughness is less than or equal to the number of cards in your graveyard.");

        var target = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<ObjectSelector>(), x => x.Quantifier == Quantifier.Target);
        var its = Assert.IsType<Reference>(Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<CharacteristicOf>()).Of);
        Assert.Equal(target.Binding, its.Binding);
    }
}
