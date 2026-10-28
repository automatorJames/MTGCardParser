namespace MTGRulesIR.Tests;

/// <summary>The MTG grammar, built once from the MTGGlyphs assembly, and a lowering over it: shared by every test in the collection.</summary>
public sealed class GrammarFixture
{
    public GlyphGrammar Grammar { get; } = new(typeof(MTGGlyphs.GlyphDefinitions.ICardSpecific).Assembly.GetTypes());

    public CorpusLowering Lowering { get; } = new();

    /// <summary>One card's text, tokenized and lowered. The name is "Sample", so write <c>{this}</c> for the card's own name.</summary>
    public CardRules Lower(string text, string types = "Creature")
    {
        var card = new Card { Name = "Sample", Text = text, Types = types };
        return Lowering.Lower(new ProcessedDocument(card, Grammar));
    }

    /// <summary>The one ability a one-line text lowers to.</summary>
    public Ability LowerAbility(string text, string types = "Creature") =>
        Assert.Single(Assert.Single(Lower(text, types).Lines).Abilities);
}

[CollectionDefinition(nameof(GrammarCollection))]
public sealed class GrammarCollection : ICollectionFixture<GrammarFixture>;
