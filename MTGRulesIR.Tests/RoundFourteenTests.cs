namespace MTGRulesIR.Tests;

/// <summary>The fourteenth round's shapes: replacements waiting for a death, two-cost abilities, power and toughness plus one, "x is …" definitions, colors as a conjoined predicate, enchanted permanents, paying and doing, and amounts plus a characteristic.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class RoundFourteenTests(GrammarFixture fixture)
{
    [Fact]
    public void WouldDieIsAZoneReplacement()
    {
        var ability = fixture.LowerAbility("if this creature would die, put it on top of its owner's library instead.");

        var replacement = Assert.IsType<Replacement>(Assert.IsType<ReplacementAbility>(ability).Effect);
        var dying = Assert.IsType<ObjectEvent>(replacement.Would);
        Assert.IsType<Self>(dying.Subject);
        Assert.Equal(["Dies"], dying.Events);
        Assert.IsType<ChangeZone>(replacement.Instead);
    }

    [Fact]
    public void EitherOfTwoCostsIsACostChoice()
    {
        var ability = Assert.IsType<ActivatedAbility>(fixture.LowerAbility("{3}, {t} or {r}, {t}: this artifact deals 1 damage to any target.", "Artifact"));

        var choice = Assert.IsType<CostChoice>(Assert.Single(ability.Costs));
        Assert.Equal(2, choice.Options.Count);
        Assert.All(choice.Options, x => Assert.Equal(2, Assert.IsType<CompoundCost>(x).Parts.Count));
        Assert.IsType<DealDamage>(ability.Effect);
    }

    [Fact]
    public void PowerAndToughnessPlusOneAreTwoStatSettings()
    {
        var statics = Assert.IsType<StaticAbility>(fixture.LowerAbility("{this}'s power is equal to the number of creature cards in all graveyards and its toughness is equal to that number plus 1."));

        var steps = Assert.IsType<Sequence>(statics.Effect).Steps;
        var power = Assert.IsType<SetStats>(steps[0]);
        var toughness = Assert.IsType<SetStats>(steps[1]);
        Assert.Equal("Power", power.Which);
        Assert.Equal("Toughness", toughness.Which);
        Assert.Equal(new Fixed(1), Assert.IsType<Adjusted>(toughness.Amount).Adjustment);
    }

    [Fact]
    public void XIsDefinedAfterTheAbilityJoinsIt()
    {
        var rules = fixture.Lower("{x}, {t}: draw a card. x is the number of cards in an opponent's hand.", "Artifact");

        var ability = Assert.IsType<ActivatedAbility>(Assert.Single(rules.Lines[0].Abilities));
        var let = Assert.IsType<LetVariable>(ability.Effect);
        Assert.Equal("X", let.Name);
        Assert.IsType<Draw>(let.Effect);
        Assert.IsType<CountOf>(let.Definition);
    }

    [Fact]
    public void AreBlackAndAreZombiesSetsTheColor()
    {
        var statics = Assert.IsType<StaticAbility>(fixture.LowerAbility("all goblins are black and are zombies in addition to their other creature types.", "Enchantment"));

        var becomes = MTGRulesIR.Lowering.NodeWalker.Nodes(statics).OfType<BecomesAspect>().ToList();
        Assert.Contains(becomes, x => x.Aspects.Any(a => a.Value == "Black") && !x.InAddition);
    }

    [Fact]
    public void ThatAreEnchantedIsAQualifier()
    {
        var statics = Assert.IsType<StaticAbility>(fixture.LowerAbility("creatures you control that are enchanted get +1/+1.", "Enchantment"));

        var creatures = Assert.IsType<ObjectSelector>(Assert.IsType<Pump>(statics.Effect).Objects);
        Assert.Contains(creatures.Filter.Qualifiers, x => x.Vocabulary == "AttachedBy" && x.Quality == "Enchanted");
    }

    [Fact]
    public void PayAndDiscardIsACompoundPayment()
    {
        var ability = fixture.LowerAbility("whenever this creature attacks, you may pay {1}{r} and discard a card. if you do, this creature deals 2 damage to any target.");

        var payment = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<PayCost>());
        var compound = Assert.IsType<CompoundCost>(payment.Cost);
        Assert.IsType<PayMana>(compound.Parts[0]);
        Assert.IsType<Discard>(Assert.IsType<ActionCost>(compound.Parts[1]).Action);
    }

    [Fact]
    public void OnePlusTheSacrificedCreaturesManaValueIsAdjusted()
    {
        var ability = fixture.LowerAbility("{1}{g/p}, {t}, sacrifice a creature: search your library for a creature card with mana value equal to 1 plus the sacrificed creature's mana value, put that card onto the battlefield, then shuffle.", "Artifact");

        var adjusted = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<Adjusted>());
        Assert.Equal(new Fixed(1), adjusted.Adjustment);
        Assert.Equal("ManaValue", Assert.IsType<CharacteristicOf>(adjusted.Base).Characteristic);
    }

    [Fact]
    public void TheNextDrawThisTurnIsANextTimeReplacement()
    {
        var ability = Assert.IsType<ActivatedAbility>(fixture.LowerAbility("{1}: the next time you would draw a card this turn, this enchantment deals 2 damage to any target instead.", "Enchantment"));

        var replacement = Assert.IsType<Replacement>(ability.Effect);
        Assert.True(replacement.NextTime);
        Assert.IsType<Draw>(Assert.IsType<WouldHappen>(replacement.Would).What);
        Assert.IsType<DealDamage>(Assert.IsType<Instead>(replacement.Instead).Effect);
    }
}
