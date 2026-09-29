namespace Glyphotype.Tests;

/// <summary>Registry-level guarantees, and a few facts the corpus relies on but can't show by itself.</summary>
[Collection(CorpusCollection.Name)]
public class RegistryTests(CorpusFixture corpus)
{
    static readonly Assembly _testAssembly = typeof(RegistryTests).Assembly;

    [Fact]
    public void Every_discovered_glyph_type_passes_structural_validation()
    {
        Assert.Empty(GlyphTypeRegistry.GetStructuralValidationErrors());
    }

    [Fact]
    public void Only_test_glyphs_are_registered()
    {
        // Guards the test project's one hard rule: reference Glyphotype only, so no other glyph assembly
        // (e.g. MTGGlyphs) joins the test grammar.
        var foreignTypes = GlyphTypeRegistry.GetAllTopLevelGlyphTypes().Where(x => x.Assembly != _testAssembly).ToList();

        Assert.Empty(foreignTypes);
    }

    [Fact]
    public void Every_test_glyph_type_is_matched_somewhere_in_the_corpus()
    {
        // Keeps the corpus comprehensive: a glyph added to the grammar without a document exercising it
        // fails here rather than going silently untested.
        var matchedTypes = corpus.ProcessedDocuments
            .SelectMany(x => x.Lines)
            .SelectMany(x => x.Glyphs)
            .SelectMany(GlyphSignature.GlyphTypesIn)
            .ToHashSet();

        var unmatchedTypes = _testAssembly.GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && typeof(Glyph).IsAssignableFrom(x))
            .Where(x => !matchedTypes.Contains(x))
            .Select(x => x.Name)
            .ToList();

        Assert.Empty(unmatchedTypes);
    }

    [Fact]
    public void TokenizationOrder_is_what_decides_between_overlapping_glyphs()
    {
        // The corpus shows BakerOpensTheShop winning "the baker opens the shop". That only proves
        // [TokenizationOrder] if OpensBuilding - tried first by default, having the longer regex - would
        // otherwise have matched the same sentence.
        var opensBuilding = GlyphTypeRegistry.GetRegexGraph(typeof(OpensBuilding));
        var bakerOpensTheShop = GlyphTypeRegistry.GetRegexGraph(typeof(BakerOpensTheShop));

        Assert.True(opensBuilding.TryMatch("the baker opens the shop", out _));
        Assert.True(opensBuilding.BuiltRegex.MinifiedRegex.Length > bakerOpensTheShop.BuiltRegex.MinifiedRegex.Length);
    }

    [Fact]
    public void Repetition_quantifiers_are_internal_to_Glyphotype()
    {
        // Repetition is what the internal primitives (CompoundOf, ManyOf) exist to express, so
        // glyph authors get no way to request it directly: not by attribute, and not through Prop().
        var glyphotype = typeof(Glyph).Assembly;

        Assert.False(glyphotype.GetType("Glyphotype.Attributes.Quantifiers.AnyNumberAttribute", throwOnError: true).IsPublic);
        Assert.False(glyphotype.GetType("Glyphotype.Attributes.Quantifiers.OneOrMoreAttribute", throwOnError: true).IsPublic);

        var publicPropOverloads = typeof(Glyph).GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x => x.Name == nameof(Glyph.Prop));
        Assert.DoesNotContain(publicPropOverloads, x => x.GetParameters().Any(p => p.ParameterType == typeof(Quantifier) || p.ParameterType == typeof(Quantifier?)));
    }

    [Theory]
    [InlineData(typeof(DayHeading), new[] { "Alternative1", "Alternative2" })]
    [InlineData(typeof(OneOf<Animal?, Person?>), new[] { "Alternative1", "Alternative2" })]
    [InlineData(typeof(Treat), new[] { "Food", "Fruit" })]
    public void OneOf_alternatives_are_found_whichever_level_declares_them(Type oneOfType, string[] expectedAlternatives)
    {
        Assert.Equal(expectedAlternatives, OneOfBase.GetAlternativeProps(oneOfType).Select(x => x.Name));
    }
}
