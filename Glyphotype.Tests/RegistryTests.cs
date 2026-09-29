namespace Glyphotype.Tests;

/// <summary>Registry-level guarantees, and a few facts the corpus relies on but can't show by itself.</summary>
[Collection(CorpusCollection.Name)]
public class RegistryTests(CorpusFixture corpus)
{
    static readonly Assembly _testAssembly = typeof(RegistryTests).Assembly;

    [Fact]
    public void Every_discovered_glyph_type_passes_structural_validation()
    {
        Assert.Empty(corpus.Grammar.GetStructuralValidationErrors());
    }

    [Fact]
    public void The_grammar_holds_only_the_types_it_was_built_from()
    {
        // Every top-level type comes from this assembly: nothing else loaded in the process (e.g. another glyph
        // assembly) leaks into a grammar built from an explicit type set.
        var foreignTypes = corpus.Grammar.TopLevelTypes.Where(x => x.Assembly != _testAssembly).ToList();

        Assert.Empty(foreignTypes);
    }

    [Fact]
    public void AllowPartialSegmentMatches_is_a_setting_of_each_grammar()
    {
        // Two grammars over the same types, differing only in this setting, coexisting in one process.
        var lenient = GlyphGrammar.FromAssemblies([_testAssembly], allowPartialSegmentMatches: true);
        const string text = "the dog sleeps in the kitchen all day.";

        Assert.Equal("«the dog sleeps in the kitchen all day» .", Signature(corpus.Grammar.Tokenize(text)));
        Assert.Equal("AnimalRests{Animal=Dog, Place=Kitchen} «all day» .", Signature(lenient.Tokenize(text)));
    }

    static string Signature(IEnumerable<CaptureUnit> units) =>
        string.Join(" ", units.Select(GlyphSignature.Of));

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
        var opensBuilding = GlyphTypeCache.GetRegexGraph(typeof(OpensBuilding));
        var bakerOpensTheShop = GlyphTypeCache.GetRegexGraph(typeof(BakerOpensTheShop));

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

    [Fact]
    public void Every_token_serializes_for_debugging()
    {
        // JsonDebug once threw for every UnmatchedString and ClauseBreak: their synthesized root traces had no
        // hydrated value, and naming its type dereferenced null.
        var failures = corpus.ProcessedDocuments
            .SelectMany(x => x.Lines)
            .SelectMany(x => x.Glyphs)
            .Select(unit => { try { _ = unit.JsonDebug; return null; } catch (Exception e) { return $"{unit.GetType().Name} \"{unit.CaptureValue}\": {e.Message}"; } })
            .Where(x => x != null)
            .ToList();

        Assert.Empty(failures);
    }

    [Fact]
    public void Start_char_sets_never_rule_out_a_position_where_a_match_begins()
    {
        // The Tokenizer skips a type wherever its StartChars says a match can't begin, without running its
        // regex - so a set missing a character would silently lose matches. Checked exhaustively: every test
        // type's regex, anchored at every position of every corpus line.
        var violations = new List<string>();

        foreach (var type in corpus.Grammar.Types)
        {
            var graph = GlyphTypeCache.GetRegexGraph(type);
            var anchored = new System.Text.RegularExpressions.Regex($@"\G({graph.BuiltRegex.MinifiedRegex})", System.Text.RegularExpressions.RegexOptions.ExplicitCapture);

            foreach (var text in corpus.ProcessedDocuments.SelectMany(x => x.Lines).Select(x => x.SourceText.FormattedText))
                for (int i = 0; i < text.Length; i++)
                    if (!graph.StartChars.CanStartWith(text[i]) && anchored.Match(text, i) is { Success: true, Length: > 0 })
                        violations.Add($"{type.Name} (starts: {graph.StartChars}) matches at \"{text[i..]}\"");
        }

        Assert.Empty(violations);
    }

    [Theory]
    [InlineData(typeof(AnimalRests), "t")]              // literal first nib
    [InlineData(typeof(FruitInBowl), "t")]
    [InlineData(typeof(KnocksOnTheDoor), "ot")]         // one-of over glyphs: "our neighbor", "the ..."
    [InlineData(typeof(ShoppingList), "befs")]          // compound of an enum: its members' initials
    [InlineData(typeof(PressToAct), "p")]
    [InlineData(typeof(IfWeather), "i")]                // a dynamic later in the sequence doesn't matter
    public void Start_char_sets_are_selective(Type type, string expected)
    {
        Assert.Equal(expected, GlyphTypeCache.GetRegexGraph(type).StartChars.ToString());
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
