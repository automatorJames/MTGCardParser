using Glyphotype.Definitions;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary>
/// <see cref="Nib.This"/>: the document's reference to itself, matched and captured as a <see cref="This"/> with no
/// property to hold it - and the only way to write it, the token as text being refused (see <see cref="Glyph.GetThisTokenError"/>).
/// </summary>
public class ThisNibTests
{
    static GlyphGrammar Build(string source)
    {
        var read = GlyphSourceReader.Read(source);
        var definition = new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers };

        return GlyphGrammar.FromDefinition(definition, allowPartialClauseMatches: false);
    }

    const string _grammar = """
        public class Wakes : Glyph
        {
            public override Nib[] Nibs => [Nib.This, "wakes up"];
        }

        public class Sleeps : Glyph
        {
            public override Nib[] Nibs => [Prop(Sleeper), "sleeps all day"];

            public It Sleeper { get; set; }
        }

        public class Mirror : Glyph
        {
            public override Nib[] Nibs => [Nib.This, "sees", Nib.This];
        }
        """;

    [Fact]
    public void It_is_captured_as_a_This_named_for_it()
    {
        var wakes = Assert.IsType<Glyph>(Assert.Single(Build(_grammar).Tokenize("{this} wakes up")), exactMatch: false);
        var capture = Assert.Single(wakes.CaptureContext.RootCaptureTrace.Children);

        Assert.Equal("This", capture.Name);
        Assert.Equal("{this}", capture.CaptureValue);
        Assert.IsType<This>(capture.ClrValue);
    }

    [Fact]
    public void More_than_one_in_a_glyph_are_numbered()
    {
        var mirror = Assert.IsType<Glyph>(Assert.Single(Build(_grammar).Tokenize("{this} sees {this}")), exactMatch: false);

        Assert.Equal(["This1", "This2"], mirror.CaptureContext.RootCaptureTrace.Children.Select(x => x.Name));
    }

    [Fact]
    public void It_is_a_referent()
    {
        var line = Assert.Single(ProcessedLine.GetAll(new TestDocument("rex", "rex wakes up. it sleeps all day.", []), Build(_grammar)));
        var resolution = Assert.Single(line.BackReferences);

        Assert.True(resolution.Antecedent.IsSelf);
        Assert.Equal("{this}", resolution.Antecedent.Text);
    }

    [Fact]
    public void It_survives_source_and_definitions()
    {
        var definition = Build(_grammar).ToDefinition();
        var wakes = definition.Glyphs.Single(x => x.Name == "Wakes");

        Assert.IsType<NibDefinition.This>(wakes.Nibs[0]);
        Assert.Contains("[Nib.This, \"wakes up\"]", GlyphSourceWriter.Write(definition, "Written"));
        Assert.Equal(definition.ToJson(), GrammarDefinition.FromJson(definition.ToJson()).ToJson());
    }

    [Theory]
    [InlineData("""public override Nib[] Nibs => ["{this} wakes up"];""")]
    [InlineData("""public override Nib[] Nibs => [Alt("{this} wakes", "{this} rises")];""")]
    [InlineData("""public override Nib[] Nibs => [Pattern(@"\{this} wakes")];""")]
    public void The_token_written_as_text_is_refused(string nibs)
    {
        var exception = Assert.ThrowsAny<Exception>(() => Build($$"""
            public class Wakes : Glyph
            {
                {{nibs}}
            }
            """));

        Assert.Contains("Nib.This", exception.Message);
    }

    [Fact]
    public void The_token_in_a_property_pattern_is_refused()
    {
        var exception = Assert.ThrowsAny<Exception>(() => Build("""
            public class Wakes : Glyph
            {
                public override Nib[] Nibs => ["it wakes", Prop(Gladly)];

                [RegexPattern("next to {this}")]
                public bool Gladly { get; set; }
            }
            """));

        Assert.Contains("Nib.This", exception.Message);
    }

    [Fact]
    public void It_can_be_neither_optional_nor_plural()
    {
        Assert.Throws<ArgumentException>(() => new OptionalNib(Nib.This));
        Assert.Throws<ArgumentException>(() => new PluralNib(Nib.This));
    }
}
