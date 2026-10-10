using Glyphotype.Definitions;
using Glyphotype.Distiller.Inspection;
using Glyphotype.Distiller.Scoring;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary>
/// Text a grammar holds without spelling it out: what open-ended patterns match (which the score must pay to spell),
/// and what a dynamic marked <see cref="AllowUnmatchedAttribute"/> leaves unresolved (which stays unmatched text).
/// </summary>
[Collection(CorpusCollection.Name)]
public class OpenTextTests(CorpusFixture corpus)
{
    [Theory]
    [InlineData("an?", false)]
    [InlineData("cards?", false)]
    [InlineData("we (wash|dry) the dishes", false)]
    [InlineData("[ ]", false)]
    [InlineData(@"\.", false)]
    [InlineData(@"\{t\}", false)]
    [InlineData("x{2}", false)]
    [InlineData(@"[^.]+", true)]
    [InlineData(@"\w+", true)]
    [InlineData(@"\d", true)]
    [InlineData("a.c", true)]
    [InlineData("ha*", true)]
    [InlineData("[abc]", true)]
    [InlineData("x{2,}", true)]
    public void A_regex_is_open_ended_when_it_can_match_text_it_never_spells(string pattern, bool isOpen)
    {
        Assert.Equal(isOpen, RegexOpenness.IsOpenEnded(pattern));
    }

    [Fact]
    public void What_a_pattern_matched_is_found_within_the_match()
    {
        // WeHaveAn: ["we have", Pattern("an?"), Prop(Traits), Prop(Animal)]
        var match = corpus.Grammar.Tokenize("we have an old dog").OfType<Glyph>().Single();
        var root = match.CaptureContext.RootCaptureTrace;

        var captures = GlyphTypeCache.GetRegexGraph(match.Type).BuiltRegex.FindPatternCaptures(root.CaptureContext.SourceText, root.Index, root.Length);

        var capture = Assert.Single(captures);
        Assert.Equal("an?", capture.Pattern);
        Assert.Equal("an", root.CaptureContext.SourceText.Substring(capture.Index, capture.Length));
    }

    [Fact]
    public void Text_an_open_ended_pattern_swallows_is_spelled_out_once_per_distinct_text()
    {
        var catchAll = GlyphGrammar.FromDefinition(new GrammarDefinition
        {
            Glyphs = [new() { Name = "MyDogHas", Nibs = [new NibDefinition.Literal("my dog has"), new NibDefinition.Pattern(@"[^.]+")] }],
        }, allowPartialClauseMatches: false);

        string[] distinct = [.. Enumerable.Range(0, 20).Select(i => $"parasite{(char)('a' + i)}ish")];
        var same = Score(catchAll, Enumerable.Repeat("my dog has parasiteaish.", 20));
        var varied = Score(catchAll, distinct.Select(x => $"my dog has {x}."));

        // Every distinct swallowed text costs at least its spelling: the grammar never wrote any of them down.
        var spelling = distinct.Skip(1).Sum(x => (x.Length + 1) * varied.CharBits);

        Assert.True(varied.DataBits - same.DataBits >= spelling, $"{varied.DataBits - same.DataBits:N0} bits more for {distinct.Length} distinct texts, which spell to {spelling:N0}");
        Assert.Equal(0, varied.UnlocatedPatternMatches);
    }

    [Fact]
    public void The_test_corpus_has_no_pattern_text_the_scorer_couldnt_locate()
    {
        Assert.Equal(0, MdlScorer.Score(corpus.Grammar, corpus.ProcessedDocuments).UnlocatedPatternMatches);
    }

    const string _whenever = """
        public class WheneverWeCan : Glyph
        {
            public override Nib[] Nibs => ["whenever we can,", Prop(Outcome)];

            [AllowUnmatched]
            public DynamicGlyph Outcome { get; set; }
        }
        """;

    GlyphGrammar WithWhenever(string source = _whenever)
    {
        var glyph = Assert.Single(GlyphSourceReader.Read(source, corpus.Grammar.ToDefinition()).Glyphs);
        return GlyphGrammar.FromDefinition(corpus.Grammar.ToDefinition().WithGlyph(glyph), allowPartialClauseMatches: false);
    }

    [Fact]
    public void A_dynamic_allowed_to_keep_unmatched_text_holds_what_nothing_resolves()
    {
        var grammar = WithWhenever();
        var lines = CorpusFixture.Process(grammar, [new TestDocument(TestDocument.Unnamed, "whenever we can, the dog snores.\nwhenever we can, the dog sleeps in the kitchen.", [])])
            .Single().Lines;

        Assert.Equal("⟦WheneverWeCan: whenever we can, ⟦Outcome: «the dog snores»⟧⟧ .", ParseRenderer.Render(lines[0], nested: true));
        Assert.StartsWith("⟦WheneverWeCan: whenever we can, ⟦Outcome→AnimalRests:", ParseRenderer.Render(lines[1], nested: true));

        // What's held unresolved is still unmatched text: it's not coverage, and it's what a residual hot spot lists.
        Assert.Equal(3, lines[0].CapturedWordCount);
        Assert.Equal(9, lines[1].CapturedWordCount);

        var match = lines[0].Glyphs.First();
        Assert.Equal("the dog snores", Assert.Single(match.UnresolvedTraces).CaptureValue);
        Assert.Equal(match.CaptureValue, CorpusEncoding.Reconstruct(match));
    }

    [Fact]
    public void The_score_counts_held_words_and_the_lines_left_with_none()
    {
        var grammar = WithWhenever();
        var documents = CorpusFixture.Process(grammar, [new TestDocument(TestDocument.Unnamed, "whenever we can, the dog snores.\nwhenever we can, the dog sleeps in the kitchen.", [])]);

        var score = MdlScorer.Score(grammar, documents);

        // The first line's frame holds "the dog snores"; the second's resolves, so that line is done.
        Assert.Equal(3, score.HeldWords);
        Assert.Equal(1, score.FullyCoveredLines);
    }

    [Fact]
    public void Without_the_attribute_an_unresolvable_dynamic_fails_the_whole_match()
    {
        var grammar = WithWhenever(_whenever.Replace("[AllowUnmatched]", ""));

        Assert.Equal("«whenever we can, the dog snores»", ParseRenderer.Render(grammar.Tokenize("whenever we can, the dog snores"), nested: true));
    }

    [Fact]
    public void Held_text_is_scored_as_unmatched_text_so_the_frame_around_it_pays_for_itself()
    {
        var outcomes = new[] { "the dog snores", "the cat purrs loudly", "the bird hums" };
        var documents = Enumerable.Range(0, 30).Select(i => $"whenever we can, {outcomes[i % outcomes.Length]}.");

        var without = Score(corpus.Grammar, documents);
        var with = Score(WithWhenever(), documents);

        // The frame's own three words are covered; the held text isn't.
        Assert.Equal(0, without.CapturedWords);
        Assert.Equal(30 * 3, with.CapturedWords);
        Assert.Contains(with.Residuals, x => x.Text == "the dog snores" && x.Occurrences == 10);
        Assert.True(with.TotalBits < without.TotalBits, $"with the frame {with.TotalBits:N0} vs without {without.TotalBits:N0}");
    }

    [Fact]
    public void Only_a_dynamic_can_keep_unmatched_text()
    {
        var definition = corpus.Grammar.ToDefinition();
        var misplaced = Assert.Single(GlyphSourceReader.Read("""
            public class Misplaced : Glyph
            {
                public override Nib[] Nibs => ["misplaced", Prop(Animal)];

                [AllowUnmatched]
                public Animal Animal { get; set; }
            }
            """, definition).Glyphs);

        var exception = Assert.ThrowsAny<Exception>(() => GlyphGrammar.FromDefinition(definition.WithGlyph(misplaced), allowPartialClauseMatches: false));
        Assert.Contains("[AllowUnmatched]", exception.Message);
    }

    [Fact]
    public void The_attribute_survives_every_trip_through_a_definition()
    {
        var glyph = Assert.Single(GlyphSourceReader.Read(_whenever, corpus.Grammar.ToDefinition()).Glyphs);
        Assert.True(glyph.Properties.Single().AllowsUnmatched);

        Assert.Contains("[AllowUnmatched]", GlyphSourceWriter.WriteGlyph(glyph));

        var emitted = GrammarEmitter.Emit(corpus.Grammar.ToDefinition().WithGlyph(glyph));
        Assert.True(GrammarDefinition.FromTypes(emitted).Glyphs.Single(x => x.Name == "WheneverWeCan").Properties.Single().AllowsUnmatched);
    }

    static MdlScore Score(GlyphGrammar grammar, IEnumerable<string> texts) =>
        MdlScorer.Score(grammar, CorpusFixture.Process(grammar, texts.Select(x => new TestDocument(TestDocument.Unnamed, x, []))));
}
