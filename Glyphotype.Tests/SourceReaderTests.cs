using Glyphotype.Definitions;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary><see cref="GlyphSourceReader"/>: C# source, read as definitions without compiling it.</summary>
[Collection(CorpusCollection.Name)]
public class SourceReaderTests(CorpusFixture corpus)
{
    GrammarDefinition Definition => corpus.Grammar.ToDefinition();

    [Fact]
    public void Written_source_reads_back_as_the_same_definition()
    {
        var read = GlyphSourceReader.Read(GlyphSourceWriter.Write(Definition, "Written"));

        var roundTripped = new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers };

        Assert.Empty(DefinitionDiff.Compare(Definition, roundTripped));
    }

    [Fact]
    public void Hand_written_source_reads_as_it_compiles()
    {
        const string source = """
            [OptionalPlural]
            public enum Pet
            {
                [RegexPattern("dog", "hound")]
                Dog,

                Cat = 5,
            }

            public interface IChore
            {
            }

            [Dependent]
            [TokenizationOrder(3)]
            public class PetNaps : Glyph, IChore
            {
                public override Joiner Joiner => Joiner.Space;
                public override Nib[] Nibs => ["the", Prop(Pet), Alt("naps", "sleeps"), Opt("soundly"), Prop(Hours), Plural(Pattern(@"\d+"))];

                public Pet Pet { get; set; }

                [Optional]
                [RegexPattern("for hours")]
                public bool Hours { get; set; }

                [TypeFilter(typeof(IChore))]
                public DynamicGlyph Next { get; set; }

                public OneOf<Pet?, PetNaps> Either { get; set; }
            }
            """;

        var read = GlyphSourceReader.Read(source);
        var compiled = GrammarDefinition.FromTypes(SourceCompiler.Compile("namespace Read;\n" + source).GetTypes());

        Assert.Equal(compiled.ToJson(), new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers }.ToJson());
    }

    [Fact]
    public void Type_names_resolve_against_the_context_grammar()
    {
        var read = GlyphSourceReader.Read("""
            public class AnimalNaps : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal), "naps", Prop(Day)];

                public Animal Animal { get; set; }
                public OnDay Day { get; set; }
            }
            """, Definition);

        var glyph = Assert.Single(read.Glyphs);
        Assert.Equal(TypeReference.Vocabulary(nameof(Animal)), glyph.Properties[0].Type);
        Assert.Equal(TypeReference.Glyph(nameof(OnDay)), glyph.Properties[1].Type);

        // Put in front of the corpus, it's a grammar like any other.
        var grammar = GlyphGrammar.FromDefinition(Definition.WithGlyph(glyph), allowPartialClauseMatches: false);
        Assert.Contains(grammar.Tokenize("the cat naps on monday"), x => x.Type.Name == "AnimalNaps");
    }

    [Fact]
    public void Everything_a_definition_cant_hold_is_reported_at_once()
    {
        var exception = Assert.Throws<GlyphSourceException>(() => GlyphSourceReader.Read("""
            [Serializable]
            public class Busy : Glyph
            {
                public override Nib[] Nibs => ["busy", Prop(Missing), Upper("x")];

                public string Name { get; set; }
                public int Computed => 3;
                public void Work() { }
            }
            """));

        Assert.Collection(exception.Errors,
            x => Assert.Contains("[Serializable]", x),
            x => Assert.Contains("Upper(\"x\")", x),
            x => Assert.Contains("'string'", x),
            x => Assert.Contains("Busy.Computed", x),
            x => Assert.Contains("MethodDeclaration", x),
            x => Assert.Contains("Prop(Missing)", x));
        Assert.All(exception.Errors, x => Assert.StartsWith("line ", x));
    }

    [Fact]
    public void Syntax_errors_are_reported_with_their_lines()
    {
        var exception = Assert.Throws<GlyphSourceException>(() => GlyphSourceReader.Read("public class Broken : Glyph\n{\n    public int X { get; set; \n}"));

        Assert.All(exception.Errors, x => Assert.Matches(@"^line \d+: ", x));
    }

    [Fact]
    public void Plural_takes_the_nib_it_makes_plural()
    {
        var exception = Assert.Throws<GlyphSourceException>(() => GlyphSourceReader.Read("""
            public class FeedsAll : Glyph { public override Nib[] Nibs => ["feed all the", Prop(Animal), Plural()]; public Animal Animal { get; set; } }
            """, corpus.Grammar.ToDefinition()));

        Assert.Contains(exception.Errors, x => x.Contains("Plural takes the nib it makes plural"));
    }

    [Fact]
    public void A_plural_saved_as_a_suffix_loads_wrapping_the_nib_before_it()
    {
        var grammar = GrammarDefinition.FromJson("""
            { "Glyphs": [ { "Name": "FeedsAll", "Kind": "Glyph",
              "Nibs": [ { "$nib": "literal", "Text": "feed all the" }, { "$nib": "prop", "Name": "Animal" }, { "$nib": "plural" } ],
              "Properties": [ { "Name": "Animal", "Type": { "Kind": "Vocabulary", "Name": "Animal" } } ] } ] }
            """);

        Assert.Equal([new NibDefinition.Literal("feed all the"), new NibDefinition.Plural(new NibDefinition.Property("Animal"))], grammar.Glyphs[0].Nibs);
    }
}
