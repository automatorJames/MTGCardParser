using Glyphotype.Definitions;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary>
/// The period rules (see <see cref="Glyph.GetPeriodError"/>): every period on a line stays a <see cref="ClauseBreak"/>
/// the Tokenizer can see - a literal nib's own period is split out into a clause-break nib, and nothing else may
/// match one.
/// </summary>
public class PeriodRuleTests
{
    static GlyphGrammar Build(string source, bool allowPeriodsInLiteralNibs = true)
    {
        var read = GlyphSourceReader.Read(source);
        var definition = new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers };

        return GlyphGrammar.FromDefinition(definition, allowPartialClauseMatches: false, allowPeriodsInLiteralNibs: allowPeriodsInLiteralNibs);
    }

    static string Signature(IEnumerable<CaptureUnit> units) =>
        string.Join(" ", units.Select(GlyphSignature.Of));

    [Fact]
    public void A_literal_nib_is_split_around_each_period_keeping_the_spaces_it_was_written_with()
    {
        var pieces = ClauseBreak.SplitAtPeriods(new Nib("by it. they can't. ever")).Select(x => x.Text);

        Assert.Equal(["by it", ".", " they can't", ".", " ever"], pieces);
    }

    [Fact]
    public void Only_a_plain_literal_nib_is_split()
    {
        Assert.Single(ClauseBreak.SplitAtPeriods(new Nib(".")));
        Assert.Single(ClauseBreak.SplitAtPeriods(new PatternNib(@"a\.b")));
        Assert.Single(ClauseBreak.SplitAtPeriods(new OptionalNib(new Nib("a. b"))));
    }

    [Fact]
    public void A_split_nib_spans_clauses_under_any_joiner()
    {
        var grammar = Build("""
            public class Tight : Glyph
            {
                public override Joiner Joiner => Joiner.None;
                public override Nib[] Nibs => ["it ends. so it goes"];
            }
            """);

        Assert.Equal("Tight .", Signature(grammar.Tokenize("it ends. so it goes.")));
    }

    [Fact]
    public void A_period_inside_a_literal_nib_is_refused_when_the_grammar_disallows_it()
    {
        const string source = """
            public class TwoClauses : Glyph
            {
                public override Nib[] Nibs => ["it ends. so it goes"];
            }
            """;

        var exception = Assert.ThrowsAny<Exception>(() => Build(source, allowPeriodsInLiteralNibs: false));

        Assert.Contains(nameof(GlobalSettings.AllowPeriodsInLiteralNibs), exception.Message);
        Assert.Equal("TwoClauses .", Signature(Build(source).Tokenize("it ends. so it goes.")));
    }

    [Theory]
    [InlineData("""["it ends. so it goes."]""")]
    [InlineData("""["it ends. so it goes", "."]""")]
    public void A_top_level_glyphs_closing_period_is_dropped_leaving_the_break_to_the_tokenizer(string nibs)
    {
        var grammar = Build($$"""
            public class Closing : Glyph
            {
                public override Nib[] Nibs => {{nibs}};
            }
            """);

        Assert.Equal("Closing .", Signature(grammar.Tokenize("it ends. so it goes.")));
        Assert.Equal("Closing", Signature(grammar.Tokenize("it ends. so it goes")));
    }

    [Theory]
    [InlineData("""
        [Dependent]
        public class Closing : Glyph { public override Nib[] Nibs => ["it ends."]; }
        public class Outer : Glyph { public override Nib[] Nibs => [Prop(Closing), "so it goes"]; public Closing Closing { get; set; } }
        """, "it's [Dependent]")]
    [InlineData("""
        [AllowPartialClauseMatch]
        public class Closing : Glyph { public override Nib[] Nibs => ["it ends."]; }
        """, "it's [AllowPartialClauseMatch]")]
    [InlineData("""
        public class Closing : Glyph { public override Nib[] Nibs => ["it ends."]; }
        public class Outer : Glyph { public override Nib[] Nibs => [Prop(Closing), "so it goes"]; public Closing Closing { get; set; } }
        """, "another glyph uses it as a property")]
    public void A_closing_period_that_means_something_is_refused(string source, string expectedError)
    {
        var exception = Assert.ThrowsAny<Exception>(() => Build(source));

        Assert.Contains(expectedError, exception.Message);
    }

    [Theory]
    [InlineData("""["the", Alt("end.", "close"), "now"]""", "an alternation can't be split")]
    [InlineData("""["the", Opt("end."), "now"]""", "can't be optional")]
    [InlineData("""["the", Pattern(@"end\.?"), "now"]""", "matches a literal period")]
    public void Nothing_but_a_literal_nib_may_hold_a_period(string nibs, string expectedError)
    {
        var exception = Assert.ThrowsAny<Exception>(() => Build($$"""
            public class Offending : Glyph
            {
                public override Nib[] Nibs => {{nibs}};
            }
            """));

        Assert.Contains(expectedError, exception.Message);
    }

    [Fact]
    public void A_vocabulary_pattern_may_not_match_a_period_either()
    {
        var exception = Assert.ThrowsAny<Exception>(() => Build("""
            public enum Title { [RegexPattern(@"mr\.")] Mister, [RegexPattern("dr")] Doctor }

            public class Greets : Glyph
            {
                public override Nib[] Nibs => ["hello", Prop(Title), "smith"];
                public Title Title { get; set; }
            }
            """));

        Assert.Contains("matches a literal period", exception.Message);
    }
}
