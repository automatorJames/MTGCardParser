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
}
