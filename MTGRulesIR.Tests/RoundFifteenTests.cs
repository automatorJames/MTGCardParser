namespace MTGRulesIR.Tests;

/// <summary>The fifteenth round's shapes: equip for a kind, counted players, the nth counter, "the difference", cards from a zone named "it", and qualified targeting.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class RoundFifteenTests(GrammarFixture fixture)
{
    [Fact]
    public void EquipForAKindKeepsTheKindAndCost()
    {
        var rules = fixture.Lower("equip soldier {w}", "Artifact");

        var keyword = Assert.Single(Assert.IsType<KeywordLine>(Assert.Single(rules.Lines[0].Abilities)).Keywords);
        Assert.Equal("Equip", keyword.Keyword);
        Assert.Equal("soldier", keyword.Parameter);
        Assert.Equal(["{w}"], Assert.IsType<PayMana>(keyword.Cost).Symbols);
    }

    [Fact]
    public void EachOpponentWhoLostLifeIsACountedPlayer()
    {
        var ability = fixture.LowerAbility("whenever this creature attacks, draw a card for each opponent who lost life this turn.");

        var players = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(ability).OfType<ObjectSelector>(), x => x.Filter?.Kinds?.Any(k => k.Value == "Player") == true);
        Assert.Equal(Quantifier.Each, players.Quantifier);
        Assert.Contains(players.Filter.Qualifiers, x => x.Quality == "Opponent");
        Assert.IsType<Satisfying>(Assert.Single(players.Filter.With));
    }

    [Fact]
    public void TheSeventhCounterIsACountedSpec()
    {
        var ability = Assert.IsType<TriggeredAbility>(fixture.LowerAbility("when the seventh plan counter is put on this enchantment, sacrifice it.", "Enchantment"));

        var counters = Assert.IsType<ObjectEvent>(ability.Trigger).Counters;
        Assert.Equal("plan", counters.Kind);
        Assert.Equal(new Fixed(7), counters.Count);
        Assert.Equal("Nth", counters.Prefix);
    }

    [Fact]
    public void TheDifferenceIsASpecialAmount()
    {
        var rules = fixture.Lower("if target opponent has more cards in hand than you, draw cards equal to the difference.", "Sorcery");

        var draw = Assert.Single(MTGRulesIR.Lowering.NodeWalker.Nodes(rules.Lines[0].Abilities[0]).OfType<Draw>());
        Assert.Equal(new Special("TheDifference"), draw.Count);
    }

    [Fact]
    public void CardsFromItAreCardsFromThatHand()
    {
        var rules = fixture.Lower("exile target creature. if that creature was a god, its controller reveals their hand and exiles all cards from it with the same name as that creature.", "Instant");

        var exiled = MTGRulesIR.Lowering.NodeWalker.Nodes(rules.Lines[0].Abilities[^1]).OfType<Exile>().Select(x => x.Objects).OfType<ObjectSelector>().Last();
        Assert.Equal("Hand", exiled.Filter.Zone.Zone);
        Assert.Contains(exiled.Filter.With, x => x is SharesCharacteristic { Characteristic: "Name" });
    }

    [Fact]
    public void AuraSpellsAreAQualifiedTargetingKind()
    {
        var statics = Assert.IsType<StaticAbility>(fixture.LowerAbility("{this} can't be the target of aura spells."));

        var cant = Assert.IsType<CantBeTargeted>(statics.Effect);
        Assert.Equal(["Spells"], cant.By);
        Assert.Contains(cant.ByQualifiers, x => x.Quality == "Aura");
    }
}
