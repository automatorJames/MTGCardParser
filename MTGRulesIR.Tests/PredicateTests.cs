namespace MTGRulesIR.Tests;

/// <summary>Filter narrowings land in the typed Predicate family, with the relation said where the narrowing is a selector.</summary>
[Collection(nameof(GrammarCollection))]
public sealed class PredicateTests(GrammarFixture fixture)
{
    static ObjectFilter FilterOfFirstSelector(Ability ability) =>
        NodeWalker.Nodes(ability).OfType<ObjectSelector>().First().Filter;

    [Fact]
    public void CountersOnThisAreRelatedOn()
    {
        var ability = fixture.LowerAbility("{t}: add {b} for each charge counter on this artifact", "Artifact");

        var filter = NodeWalker.Nodes(ability).OfType<ObjectSelector>().First(x => x.Filter.Kinds?.Contains("counter") == true).Filter;
        var related = Assert.IsType<Related>(Assert.Single(filter.With));
        Assert.Equal(Relation.On, related.Relation);
        Assert.IsType<Self>(related.To);
    }

    [Fact]
    public void CardsFromAmongThemAreRelatedAmong()
    {
        var rules = fixture.Lower("reveal the top four cards of your library. put a creature card from among them into your hand", "Sorcery");

        var among = rules.Lines[0].Abilities.SelectMany(NodeWalker.Nodes).OfType<Related>().Single(x => x.Relation == Relation.Among);
        Assert.IsType<Reference>(among.To);
    }

    [Fact]
    public void WithKeywordsIsAPredicate()
    {
        var ability = fixture.LowerAbility("destroy target creature with flying", "Instant");

        var filter = FilterOfFirstSelector(ability);
        var has = Assert.IsType<HasKeywords>(Assert.Single(filter.With));
        Assert.Equal(["Flying"], has.Keywords);
    }

    [Fact]
    public void OfTheChosenTypeIsOfChosen()
    {
        var ability = fixture.LowerAbility("creatures you control of the chosen type get +1/+1", "Enchantment");

        var filter = FilterOfFirstSelector(ability);
        Assert.Equal("OfTheChosenType", Assert.IsType<OfChosen>(Assert.Single(filter.With)).Characteristic);
    }

    [Fact]
    public void SpellCastDuringATurnIsCastDuring()
    {
        var ability = fixture.LowerAbility("whenever you cast a spell during an opponent's turn, draw a card", "Enchantment");

        var during = NodeWalker.Nodes(ability).OfType<CastDuring>().Single();
        Assert.NotNull(during.Phase);
    }

    [Fact]
    public void NarrowingWithoutARuleIsAnOpaquePredicate()
    {
        var rules = fixture.Lower("creatures blocking or blocked by this creature have lifelink");

        var line = rules.Lines[0];
        Assert.True(line.EngineClosed || !line.FullyLowered);
        Assert.DoesNotContain(line.Open ?? [], x => x.StartsWith("ObjectFilter.With"));
    }
}
