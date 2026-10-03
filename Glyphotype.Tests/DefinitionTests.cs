using Glyphotype.Definitions;

namespace Glyphotype.Tests;

/// <summary>
/// <see cref="GrammarDefinition"/>s hold everything the engine reads from a glyph type: the test grammar survives
/// every trip through them - to JSON, to emitted types, to C# source - and a grammar compiled from its definition
/// tokenizes the whole corpus exactly as the hand-written one does.
/// </summary>
[Collection(CorpusCollection.Name)]
public class DefinitionTests(CorpusFixture corpus)
{
    GrammarDefinition Definition => corpus.Grammar.ToDefinition();

    [Fact]
    public void Definitions_survive_JSON()
    {
        var json = Definition.ToJson();

        Assert.Equal(json, GrammarDefinition.FromJson(json).ToJson());
    }

    [Fact]
    public void Emitted_types_read_back_as_the_same_definition()
    {
        var emitted = GrammarEmitter.Emit(Definition);

        Assert.Equal(Definition.ToJson(), GrammarDefinition.FromTypes(emitted).ToJson());
    }

    [Fact]
    public void Written_source_compiles_back_to_the_same_definition()
    {
        var compiled = SourceCompiler.Compile(GlyphSourceWriter.Write(Definition, "Written"));

        Assert.Equal(Definition.ToJson(), GrammarDefinition.FromTypes(compiled.GetTypes()).ToJson());
    }

    [Fact]
    public void A_grammar_built_from_the_definition_tokenizes_the_corpus_identically()
    {
        // Through JSON first, so this also proves the serialized form is missing nothing.
        var definition = GrammarDefinition.FromJson(Definition.ToJson());
        var grammar = GlyphGrammar.FromDefinition(definition, allowPartialClauseMatches: false);

        var mismatches = CorpusFixture.Process(grammar)
            .Select(x => (Expected: TestCorpus.Documents.Single(y => y.Text == x.Document.Text).ExpectedLines, Actual: x.Lines.Select(GlyphSignature.Of).ToArray()))
            .Where(x => !x.Expected.SequenceEqual(x.Actual))
            .Select(x => $"expected: {string.Join(" / ", x.Expected)}\n  actual: {string.Join(" / ", x.Actual)}")
            .ToList();

        Assert.Empty(mismatches);
    }

    [Fact]
    public void A_glyph_can_be_defined_on_top_of_an_existing_grammar()
    {
        // One new glyph, referring to existing types by name - as a definition built on a working grammar does.
        var glyph = new GlyphDefinition
        {
            Name = "AnimalHides",
            Nibs = [new NibDefinition.Literal("the"), new NibDefinition.Property("Animal"), new NibDefinition.Literal("hides in the"), new NibDefinition.Property("Place")],
            Properties =
            [
                new() { Name = "Animal", Type = TypeReference.Vocabulary(nameof(Animal)) },
                new() { Name = "Place", Type = TypeReference.Vocabulary(nameof(Place)) },
            ],
        };

        var newTypes = GrammarEmitter.Emit(new GrammarDefinition { Glyphs = [glyph] }, knownTypes: corpus.Grammar.Types);
        var grammar = new GlyphGrammar(corpus.Grammar.Types.Concat(newTypes), allowPartialClauseMatches: false);

        Assert.Equal("AnimalHides{Animal=Dog, Place=Barn} .", string.Join(" ", grammar.Tokenize("the dog hides in the barn.").Select(GlyphSignature.Of)));
    }

    [Fact]
    public void Written_source_reads_like_hand_written_glyphs()
    {
        AssertWrites(nameof(AnimalRests), """
            public class AnimalRests : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal), Alt("sleeps", "naps"), "in the", Prop(Place)];

                public Animal Animal { get; set; }
                public Place Place { get; set; }
            }
            """);

        AssertWrites(nameof(Price), """
            [Dependent]
            public class Price : Glyph
            {
                public override Joiner Joiner => Joiner.None;
                public override Nib[] Nibs => ["$", Prop(Dollars)];

                public int Dollars { get; set; }
            }
            """);

        AssertWrites(nameof(PersonComes), """
            public class PersonComes : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Person), "comes", Prop(Day), Prop(Time)];

                public Person Person { get; set; }
                public OnDay Day { get; set; }
                [Optional]
                public AtTime Time { get; set; }
            }
            """);

        AssertWrites(nameof(EveryWeekday), """
            public class EveryWeekday : Glyph
            {
                public override Nib[] Nibs => ["every", Prop(Weekday), ",", Prop(Chore)];

                public Weekday Weekday { get; set; }
                [TypeFilter(typeof(IChore))]
                public DynamicGlyph Chore { get; set; }
            }
            """);

        AssertWrites(nameof(DoTheDishes), """
            [Dependent]
            [RegexPattern("we (wash|dry) the dishes")]
            public class DoTheDishes : Glyph, IChore;
            """);

        AssertWrites(nameof(DayHeading), """
            public class DayHeading : OneOf<Weekday?, Holiday?>;
            """);

        AssertWrites(nameof(TraitList), """
            [Dependent]
            [JoinedBy(Joiner.Space)]
            public class TraitList : CompoundOf<Trait>;
            """);

        AssertWrites(nameof(WeHaveAn), """
            public class WeHaveAn : Glyph
            {
                public override Nib[] Nibs => ["we have", Pattern("an?"), Prop(Traits), Prop(Animal)];

                public TraitList Traits { get; set; }
                public Animal Animal { get; set; }
            }
            """);
    }

    void AssertWrites(string glyphName, string expected) =>
        Assert.Equal(
            expected.ReplaceLineEndings().Trim(),
            GlyphSourceWriter.WriteGlyph(Definition.Glyphs.Single(x => x.Name == glyphName)).ReplaceLineEndings().Trim());
}
