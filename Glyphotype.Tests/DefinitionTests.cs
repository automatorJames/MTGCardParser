using System.Runtime.CompilerServices;
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
        // Afresh, not reusing the compiled types a workspace manager registers (see GrammarEmitter.Reuse): those would read back trivially.
        var emitted = GrammarEmitter.Emit(Definition, reuse: false);

        Assert.Equal(Definition.ToJson(), GrammarDefinition.FromTypes(emitted).ToJson());
    }

    [Fact]
    public void A_grammar_built_from_a_compiled_grammars_definition_reuses_its_types()
    {
        GrammarEmitter.Reuse(corpus.Grammar.Types);

        var emitted = GrammarEmitter.Emit(Definition);

        Assert.All(emitted, x => Assert.False(x.Assembly.IsDynamic, $"{x.Name} was emitted again"));
    }

    [Fact]
    public void A_grammar_built_from_a_definition_tries_the_same_types_as_one_compiled_from_assemblies()
    {
        var built = GlyphGrammar.FromDefinition(Definition);

        // Built-ins included: a dynamic resolves to a pronoun under either.
        Assert.Contains(typeof(It), corpus.Grammar.DependentTypes);
        Assert.Equal(corpus.Grammar.TopLevelTypes.Select(x => x.Name), built.TopLevelTypes.Select(x => x.Name));
        Assert.Equal(corpus.Grammar.DependentTypes.Select(x => x.Name), built.DependentTypes.Select(x => x.Name));
    }

    [Fact]
    public void Every_built_in_reads_as_a_definition_and_writes_as_source()
    {
        var builtIns = GrammarDefinition.BuiltIns;

        Assert.Equal(GrammarDefinition.BuiltInNames.Except([nameof(DynamicGlyph)]).Order(), builtIns.Glyphs.Select(x => x.Name).Concat(builtIns.Vocabularies.Select(x => x.Name)).Order());
        Assert.All(builtIns.Glyphs, x => Assert.NotEmpty(x.Nibs));
        Assert.Contains("public class It : BackReference", GlyphSourceWriter.Write(builtIns, "BuiltIns"));
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
        var grammar = new GlyphGrammar(GrammarEmitter.Emit(definition, reuse: false));

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
        var grammar = new GlyphGrammar(corpus.Grammar.Types.Concat(newTypes));

        Assert.Equal("AnimalHides{Animal=Dog, Place=Barn} .", string.Join(" ", grammar.Tokenize("the dog hides in the barn.").Select(GlyphSignature.Of)));
    }

    [Fact]
    public void Emitting_a_grammar_again_reuses_every_type_but_a_changed_glyph_and_the_glyphs_referring_to_it()
    {
        var hides = new GlyphDefinition
        {
            Name = "AnimalHides",
            Nibs = [new NibDefinition.Literal("the"), new NibDefinition.Property("Animal"), new NibDefinition.Literal("hides")],
            Properties = [new() { Name = "Animal", Type = TypeReference.Vocabulary(nameof(Animal)) }],
        };

        var hidesAgain = new GlyphDefinition
        {
            Name = "AnimalHidesAgain",
            Nibs = [new NibDefinition.Property("Hiding"), new NibDefinition.Literal("again")],
            Properties = [new() { Name = "Hiding", Type = TypeReference.Glyph("AnimalHides") }],
        };

        var seeks = new GlyphDefinition
        {
            Name = "AnimalSeeks",
            Nibs = [new NibDefinition.Literal("the"), new NibDefinition.Property("Animal"), new NibDefinition.Literal("seeks")],
            Properties = [new() { Name = "Animal", Type = TypeReference.Vocabulary(nameof(Animal)) }],
        };

        Dictionary<string, Type> Emit(params GlyphDefinition[] glyphs) =>
            GrammarEmitter.Emit(new GrammarDefinition { Glyphs = glyphs }, knownTypes: corpus.Grammar.Types).ToDictionary(x => x.Name);

        var first = Emit(hides, hidesAgain, seeks);
        var same = Emit(hides, hidesAgain, seeks);
        var documented = Emit(hides with { Documentation = new() { Summary = "An animal hiding." } }, hidesAgain, seeks);
        var changed = Emit(hides with { Nibs = [new NibDefinition.Literal("the"), new NibDefinition.Property("Animal"), new NibDefinition.Literal("hides away")] }, hidesAgain, seeks);

        Assert.All(first.Keys, x => Assert.Same(first[x], same[x]));
        Assert.All(first.Keys, x => Assert.Same(first[x], documented[x]));
        Assert.NotSame(first["AnimalHides"], changed["AnimalHides"]);
        Assert.NotSame(first["AnimalHidesAgain"], changed["AnimalHidesAgain"]);
        Assert.Same(first["AnimalSeeks"], changed["AnimalSeeks"]);
        Assert.Same(changed["AnimalHides"], changed["AnimalHidesAgain"].GetProperty("Hiding").PropertyType);
    }

    [Fact]
    public void An_emitted_type_is_unloaded_once_nothing_uses_it()
    {
        var emitted = EmitAndUse();

        for (int i = 0; i < 20 && emitted.IsAlive; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        Assert.False(emitted.IsAlive);
    }

    /// <summary>Emits a glyph, and tokenizes with a grammar holding it, so everything cached about it is - returning only a weak reference to it.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    WeakReference EmitAndUse()
    {
        var glyph = new GlyphDefinition
        {
            Name = "AnimalVanishes",
            Nibs = [new NibDefinition.Literal("the"), new NibDefinition.Property("Animal"), new NibDefinition.Literal("vanishes")],
            Properties = [new() { Name = "Animal", Type = TypeReference.Vocabulary(nameof(Animal)) }],
        };

        var type = GrammarEmitter.Emit(new GrammarDefinition { Glyphs = [glyph] }, knownTypes: corpus.Grammar.Types).Single();
        var grammar = new GlyphGrammar(corpus.Grammar.Types.Append(type));

        Assert.Equal("AnimalVanishes{Animal=Dog} .", string.Join(" ", grammar.Tokenize("the dog vanishes.").Select(GlyphSignature.Of)));
        return new(type);
    }

    [Fact]
    public void A_definition_wrapping_a_nib_in_what_cant_wrap_it_is_refused_as_it_is_emitted()
    {
        // As a definition read from JSON can, never having been through the source reader's own check.
        var glyph = new GlyphDefinition
        {
            Name = "AnimalHides",
            Nibs = [new NibDefinition.Literal("the"), new NibDefinition.Optional(new NibDefinition.Property("Animal")), new NibDefinition.Literal("hides")],
            Properties = [new() { Name = "Animal", Type = TypeReference.Vocabulary(nameof(Animal)) }],
        };

        var exception = Assert.Throws<InvalidOperationException>(() => GrammarEmitter.Emit(new GrammarDefinition { Glyphs = [glyph] }, knownTypes: corpus.Grammar.Types));

        Assert.Equal("AnimalHides: Opt(Prop(...)) - make a property optional with [Optional] on it instead", exception.Message);
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
