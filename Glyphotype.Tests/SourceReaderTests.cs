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
    public void A_glyphs_doc_comment_is_its_documentation_read_and_written_alike()
    {
        const string source = """
            /// <summary>The animal napping, as in <see cref="AnimalRests"/>.</summary>
            /// <exampledoc>Pet diary &amp; notes</exampledoc>
            /// <examplecapture>the cat naps</examplecapture>
            public class AnimalNaps : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal), "naps"];

                public Animal Animal { get; set; }
            }

            /// <summary>
            /// The animal dozing off,
            ///
            /// over two paragraphs.
            /// </summary>
            public class AnimalDozes : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal), "dozes"];

                public Animal Animal { get; set; }
            }

            public class AnimalSnores : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal), "snores"];

                public Animal Animal { get; set; }
            }
            """;

        var read = GlyphSourceReader.Read(source, Definition);

        Assert.Equal(new GlyphDocumentation
        {
            Summary = """The animal napping, as in <see cref="AnimalRests"/>.""",
            ExampleDocument = "Pet diary & notes",
            ExampleCapture = "the cat naps",
        }, read.Glyphs[0].Documentation);
        Assert.Equal(new GlyphDocumentation { Summary = "The animal dozing off,\n\nover two paragraphs." }, read.Glyphs[1].Documentation);
        Assert.Null(read.Glyphs[2].Documentation);

        Assert.StartsWith("/// <summary>\n/// The animal dozing off,\n///\n/// over two paragraphs.\n/// </summary>\npublic class AnimalDozes",
            GlyphSourceWriter.WriteGlyph(read.Glyphs[1]).ReplaceLineEndings("\n"));
        Assert.StartsWith("public class AnimalSnores", GlyphSourceWriter.WriteGlyph(read.Glyphs[2]));

        Assert.Equal(read.Glyphs.Select(x => x.Documentation), read.Glyphs.Select(x => GlyphSourceReader.Read(GlyphSourceWriter.WriteGlyph(x), Definition).Glyphs.Single().Documentation));
    }

    [Theory]
    [InlineData("/// <remarks>Not documentation.</remarks>\n/// <summary>Naps.</summary>", "Naps.")]
    [InlineData("/// Text outside any tag.\n/// <summary>Naps.</summary>", "Naps.")]
    [InlineData("/// <summary>Naps.</summary>\n/// <summary>Two.</summary>", "Naps.")]
    [InlineData("/// <summary>Unclosed.", null)]
    [InlineData("/// Just a note.", null)]
    public void A_doc_comment_is_read_for_whatever_documentation_it_has_and_never_refused(string comment, string summary)
    {
        var glyph = Assert.Single(GlyphSourceReader.Read(comment + "\npublic class AnimalNaps : Glyph;", Definition).Glyphs);

        Assert.Equal(summary, glyph.Documentation?.Summary);
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

    [Theory]
    [InlineData("Opt(Prop(Animal))", "Opt(Prop(...)) - make a property optional with [Optional] on it instead")]
    [InlineData("Opt(Plural(\"dog\"))", "Opt(Plural(...)) - a nib can't be both optional and plural")]
    [InlineData("Opt(Nib.This)", "Opt(Nib.This) - an embedded glyph can't be optional")]
    [InlineData("Plural(Opt(\"dog\"))", "Plural(Opt(...)) - a nib can't be both optional and plural")]
    [InlineData("Plural(Plural(\"dog\"))", "Plural(Plural(...)) - a nib can only be made plural once")]
    [InlineData("Plural(Nib.This)", "Plural(Nib.This) - an embedded glyph can't be made plural")]
    public void A_nib_wrapped_in_what_cant_wrap_it_is_refused_as_it_is_read(string nib, string expected)
    {
        var exception = Assert.Throws<GlyphSourceException>(() => GlyphSourceReader.Read($$"""
            public class FeedsAll : Glyph { public override Nib[] Nibs => ["feed all the", {{nib}}]; public Animal Animal { get; set; } }
            """, corpus.Grammar.ToDefinition()));

        Assert.Contains(exception.Errors, x => x.Contains("FeedsAll: " + expected));
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
