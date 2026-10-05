using Glyphotype.Definitions;
using Glyphotype.Distiller.Workbench;
using Glyphotype.RegexGeneration.Graph;

namespace Glyphotype.Tests;

/// <summary><see cref="Glyph.Some"/>: at least one of several literal texts, in order, joined as separate nibs would be.</summary>
public class SomeNibTests
{
    const string _vocabulary = """
        public enum Pet { Dog, Cat }
        public enum Toy { Ball, Rope }
        """;

    static GlyphGrammar Build(string glyphs)
    {
        var read = GlyphSourceReader.Read(_vocabulary + glyphs);
        var definition = new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers };

        return GlyphGrammar.FromDefinition(definition, allowPartialClauseMatches: false, allowPeriodsInLiteralNibs: true);
    }

    static string Signature(GlyphGrammar grammar, string line) =>
        string.Join(" ", grammar.Tokenize(line).Select(x => x.GetType().Name));

    [Fact]
    public void Its_regex_is_each_item_that_could_come_first_followed_by_the_rest_optionally()
    {
        Assert.Equal("(a([ ]b)?([ ]c)?|b([ ]c)?|c)", BuiltRegex.EscapeSpaces(new SomeNib("a", "b", "c").Regex));
        Assert.Equal("(a(b)?(c)?|b(c)?|c)", "(" + string.Join("|", new SomeNib("a", "b", "c").Branches(Joiner.None)) + ")");
    }

    [Fact]
    public void A_separator_of_punctuation_and_space_needs_at_least_one_of_them()
    {
        var grammar = Build("""
            public class PetAndToy : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Pet), Some(",", " "), Prop(Toy)];
                public Pet Pet { get; set; }
                public Toy Toy { get; set; }
            }
            """);

        Assert.Equal("PetAndToy", Signature(grammar, "the dog, ball"));
        Assert.Equal("PetAndToy", Signature(grammar, "the dog,ball"));
        Assert.Equal("PetAndToy", Signature(grammar, "the dog ball"));
        Assert.NotEqual("PetAndToy", Signature(grammar, "the dogball"));
        Assert.NotEqual("PetAndToy", Signature(grammar, "the dog ,ball"));
    }

    [Fact]
    public void Word_items_are_joined_by_the_glyphs_joiner()
    {
        var grammar = Build("""
            public class BigRedBall : Glyph
            {
                public override Nib[] Nibs => ["the", Some("big", "red"), "ball"];
            }
            """);

        Assert.Equal("BigRedBall", Signature(grammar, "the big ball"));
        Assert.Equal("BigRedBall", Signature(grammar, "the red ball"));
        Assert.Equal("BigRedBall", Signature(grammar, "the big red ball"));
        Assert.NotEqual("BigRedBall", Signature(grammar, "the ball"));
        Assert.NotEqual("BigRedBall", Signature(grammar, "the red big ball"));
    }

    [Fact]
    public void It_can_anchor_a_dynamic_since_it_always_matches_something()
    {
        // Opt("then"), Opt("so") in its place could both match nothing, and is refused as an anchor.
        const string source = """
            public class Chases : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Pet), "chases the", Prop(Toy)];
                public Pet Pet { get; set; }
                public Toy Toy { get; set; }
            }

            public class Follows : Glyph
            {
                public override Nib[] Nibs => [{{nibs}}, Prop(Next)];
                public DynamicGlyph Next { get; set; }
            }
            """;

        var refused = Assert.ThrowsAny<Exception>(() => Build(source.Replace("{{nibs}}", """Opt("then"), Opt("so")""")));
        Assert.Contains("nothing that is guaranteed to match", refused.Message);

        var grammar = Build(source.Replace("{{nibs}}", """Some("then", "so")"""));
        Assert.Equal("Follows", Signature(grammar, "then so the dog chases the ball"));
    }

    [Fact]
    public void It_needs_one_or_more_non_empty_items()
    {
        Assert.Throws<ArgumentException>(() => new SomeNib());
        Assert.Throws<ArgumentException>(() => new SomeNib("a", ""));
    }

    [Fact]
    public void It_round_trips_through_source_and_definitions()
    {
        var read = GlyphSourceReader.Read("""
            public class Tidy : Glyph { public override Nib[] Nibs => ["the", Some("big", "red"), "ball"]; }
            """);

        var nib = Assert.IsType<NibDefinition.Some>(read.Glyphs.Single().Nibs[1]);
        Assert.Equal(["big", "red"], nib.Texts);
        Assert.Contains("Some(\"big\", \"red\")", GlyphSourceWriter.WriteGlyph(read.Glyphs.Single()));
    }
}
