using Glyphotype.Definitions;
using Glyphotype.Distiller.Inspection;
using Glyphotype.Distiller.Scoring;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary>
/// Text a grammar holds without spelling it out: what open-ended patterns match, which the score must pay to spell -
/// and what a dynamic can't resolve, which fails its match rather than being held.
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

    [Fact]
    public void A_dynamic_nothing_resolves_fails_the_whole_match()
    {
        var glyph = Assert.Single(GlyphSourceReader.Read("""
            public class WheneverWeCan : Glyph
            {
                public override Nib[] Nibs => ["whenever we can,", Prop(Outcome)];

                public DynamicGlyph Outcome { get; set; }
            }
            """, corpus.Grammar.ToDefinition()).Glyphs);
        var grammar = GlyphGrammar.FromDefinition(corpus.Grammar.ToDefinition().WithGlyph(glyph), allowPartialClauseMatches: false);

        Assert.Equal("«whenever we can, the dog snores»", ParseRenderer.Render(grammar.Tokenize("whenever we can, the dog snores"), nested: true));
        Assert.StartsWith("⟦WheneverWeCan: whenever we can, ⟦Outcome→AnimalRests:", ParseRenderer.Render(grammar.Tokenize("whenever we can, the dog sleeps in the kitchen"), nested: true));
    }

    static MdlScore Score(GlyphGrammar grammar, IEnumerable<string> texts) =>
        MdlScorer.Score(grammar, CorpusFixture.Process(grammar, texts.Select(x => new TestDocument(TestDocument.Unnamed, x, []))));
}
