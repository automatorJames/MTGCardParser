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
    [Fact]
    public void IsACreatureWithBaseStatsIsAnAnimation()
    {
        var rules = fixture.Lower("enchanted artifact is a creature with base power and toughness 5/5 in addition to its other types.", "Enchantment");

        var statics = Assert.IsType<StaticAbility>(Assert.Single(rules.Lines[0].Abilities));
        var animate = Assert.IsType<Animate>(statics.Effect);
        Assert.Equal(Quantifier.Attached, Assert.IsType<ObjectSelector>(animate.Objects).Quantifier);
        Assert.Equal(new StatsMod(new Fixed(5), new Fixed(5)), animate.BaseStats);
        Assert.True(animate.InAddition);
        Assert.Equal(["Creature"], animate.Types);
    }

    [Fact]
    public void AssignsDamageByToughnessIsAStatic()
    {
        var rules = fixture.Lower("each creature you control assigns combat damage equal to its toughness rather than its power.", "Enchantment");

        var statics = Assert.IsType<StaticAbility>(Assert.Single(rules.Lines[0].Abilities));
        Assert.Equal(Quantifier.Each, Assert.IsType<ObjectSelector>(Assert.IsType<AssignsDamageByToughness>(statics.Effect).Objects).Quantifier);
    }

    [Fact]
    public void ATriggerUntilEndOfTurnIsALastingDelayedTrigger()
    {
        var rules = fixture.Lower("until end of turn, whenever a player taps an island for mana, that player adds an additional {u}.", "Instant");

        var lasting = Assert.IsType<Lasting>(Assert.IsType<SpellAbility>(Assert.Single(rules.Lines[0].Abilities)).Effect);
        Assert.IsType<UntilPhase>(lasting.Duration);
        var delayed = Assert.IsType<Delayed>(lasting.Effect);
        Assert.Contains("IsTappedForMana", Assert.IsType<ObjectEvent>(delayed.When).Events);
        Assert.IsType<AddMana>(delayed.Effect);
    }

    [Fact]
    public void ANamedLegendaryTokenKeepsItsName()
    {
        var ability = fixture.LowerAbility("{t}: create voja, a legendary 2/2 green and white wolf creature token.");

        var token = Assert.IsType<TokenSpec>(Assert.IsType<CreateTokens>(Assert.IsType<ActivatedAbility>(ability).Effect).Token);
        Assert.Equal("voja", token.Name);
        Assert.True(token.Legendary);
        Assert.Equal(["Wolf"], token.Subtypes);
        Assert.Equal(["Green", "White"], token.Colors);
    }

    [Fact]
    public void CastDuringYourMainPhaseIsACastCondition()
    {
        var rules = fixture.Lower("addendum — if you cast this spell during your main phase, draw a card.", "Instant");

        var cast = Assert.IsType<WasCast>(Assert.IsType<Conditional>(Assert.IsType<SpellAbility>(Assert.IsType<LabeledAbility>(Assert.Single(rules.Lines[0].Abilities)).Ability).Effect).If);
        Assert.IsType<Self>(cast.Spell);
        Assert.Equal("MainPhase", cast.During.Phase);
    }

    [Fact]
    public void AReducedMaximumHandSizeIsANegativeChange()
    {
        var statics = Assert.IsType<StaticAbility>(fixture.LowerAbility("each opponent's maximum hand size is reduced by one."));

        var hand = Assert.IsType<MaximumHandSize>(statics.Effect);
        Assert.Equal(PlayerKind.EachOpponent, Assert.IsType<PlayerSelector>(hand.Player).Kind);
        Assert.Equal(new Fixed(-1), hand.Change);
        Assert.Null(hand.Size);
    }

    [Fact]
    public void SpellsThatTargetThisCreatureAreTargeting()
    {
        var statics = Assert.IsType<StaticAbility>(fixture.LowerAbility("spells your opponents cast that target this creature cost {2} more to cast."));

        var spells = Assert.IsType<ObjectSelector>(Assert.IsType<CostModification>(statics.Effect).Spells);
        Assert.Contains(spells.Filter.With, x => x is Targeting { Target: Self });
    }

    [Fact]
    public void EitherGameStateIsAnOr()
    {
        var ability = fixture.LowerAbility("at the beginning of your end step, if a nonland permanent left the battlefield this turn or a spell was warped this turn, put a +1/+1 counter on this creature.");

        var either = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<Or>());
        Assert.Equal(2, either.Conditions.Count);
        Assert.All(either.Conditions, x => Assert.IsType<Happened>(x));
    }

    [Fact]
    public void BecomingACreatureWithAQuotedAbilityAnimatesAndGrants()
    {
        var ability = fixture.LowerAbility("{2}{r}{g}: until end of turn, this land becomes a 3/3 red and green elemental creature with \"whenever this creature attacks, put a +1/+1 counter on it.\" it's still a land.", "Land");

        var steps = MTGRulesIR.Lowering.NodeWalker.Nodes(ability).ToList();
        var animate = Assert.Single(steps.OfType<Animate>());
        Assert.Equal(new Fixed(3), animate.Power);
        Assert.Equal(["Elemental"], animate.Subtypes);
        Assert.IsType<TriggeredAbility>(Assert.Single(steps.OfType<GrantAbility>()).Ability);
    }

    [Fact]
    public void CombatDamageOfNoAmountKeepsItsKind()
    {
        var ability = fixture.LowerAbility("if this creature would deal combat damage to a player, it deals double that damage to that player instead.");

        var would = Assert.IsType<DealDamage>(Assert.IsType<WouldHappen>(Assert.IsType<Replacement>(Assert.IsType<ReplacementAbility>(ability).Effect).Would).What);
        Assert.Equal("Combat", would.Kind);
        Assert.Null(Assert.Single(would.Portions).Amount);
    }

    [Fact]
    public void ThoseTokensAgainAreThoseTokens()
    {
        var rules = fixture.Lower("create six 1/1 white kor soldier creature tokens. if this spell was kicked, create twelve of those tokens instead.", "Sorcery");

        var again = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(rules.Lines[0].Abilities[^1]).OfType<CreateTokens>(), x => x.Token is ThoseTokens);
        Assert.Equal(new Fixed(12), again.Count);
    }
}
