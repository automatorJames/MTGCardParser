using Glyphotype.Definitions;
using Glyphotype.Distiller.Scoring;

namespace Glyphotype.Tests;

/// <summary>The MDL scorer: its codes, that its corpus encoding is lossless, and that it ranks grammars the way MDL should.</summary>
[Collection(CorpusCollection.Name)]
public class ScoringTests(CorpusFixture corpus)
{
    [Fact]
    public void The_adaptive_code_is_the_sequential_Laplace_code()
    {
        // Coding "a a b a c" over a 4-symbol alphabet one choice at a time: each costs -log2((count so far + 1) / (choices so far + 4)).
        string[] sequence = ["a", "a", "b", "a", "c"];
        var counts = new Dictionary<string, int>();
        var sequential = 0.0;

        for (int i = 0; i < sequence.Length; i++)
        {
            sequential -= Math.Log2((counts.GetValueOrDefault(sequence[i]) + 1.0) / (i + 4));
            counts[sequence[i]] = counts.GetValueOrDefault(sequence[i]) + 1;
        }

        Assert.Equal(sequential, CodeLength.Adaptive(counts.Values, alphabetSize: 4), precision: 9);
    }

    [Fact]
    public void A_context_that_always_makes_the_same_choice_costs_nothing()
    {
        Assert.Equal(0, CodeLength.Adaptive([50], alphabetSize: 1), precision: 9);
    }

    [Fact]
    public void Every_match_in_the_corpus_is_rebuilt_exactly_from_its_encoding()
    {
        // The encoding codes a match as its frame plus its children's choices; if any character of a match fell
        // outside that (or into it twice), the rebuilt text would differ.
        var mismatches = corpus.ProcessedDocuments
            .SelectMany(x => x.Lines)
            .SelectMany(x => x.Glyphs)
            .Where(x => CorpusEncoding.Reconstruct(x) != x.CaptureValue)
            .Select(x => $"{x.CaptureValue} -> {CorpusEncoding.Reconstruct(x)}")
            .ToList();

        Assert.Empty(mismatches);
    }

    [Fact]
    public void The_baseline_is_the_empty_grammars_own_score()
    {
        // (The test grammar itself doesn't compress the test corpus, and shouldn't be expected to: the corpus
        // exercises each glyph a handful of times, amid near misses, for coverage rather than repetition.)
        var score = MdlScorer.Score(corpus.Grammar, corpus.ProcessedDocuments);
        var empty = Score(new GlyphGrammar([], allowPartialSegmentMatches: false), TestCorpus.Documents);

        Assert.Equal(score.GrammarBits + score.ComponentBits.Values.Sum(), score.TotalBits, precision: 6);
        Assert.Equal(empty.TotalBits, score.BaselineBits, precision: 6);
        Assert.Equal(1, empty.CompressionRatio, precision: 9);
    }

    [Fact]
    public void A_template_with_a_vocabulary_beats_a_literal_per_sentence_which_beats_no_grammar()
    {
        // "My dog has fleas" / "my dog has lice": the toy corpus from the distiller design discussion.
        string[] parasites = ["fleas", "lice", "ticks", "mites"];
        var documents = Enumerable.Range(0, 40)
            .Select(i => new TestDocument(TestDocument.Unnamed, $"my dog has {parasites[i % parasites.Length]}.", []))
            .ToList();

        var literals = new GrammarDefinition
        {
            Glyphs = parasites.Select(x => new GlyphDefinition { Name = $"MyDogHas{char.ToUpper(x[0])}{x[1..]}", Nibs = [new NibDefinition.Literal($"my dog has {x}")] }).ToList(),
        };

        var template = new GrammarDefinition
        {
            Glyphs =
            [
                new()
                {
                    Name = "MyDogHas",
                    Nibs = [new NibDefinition.Literal("my dog has"), new NibDefinition.Property("Parasite")],
                    Properties = [new() { Name = "Parasite", Type = TypeReference.Vocabulary("Parasite") }],
                },
            ],
            Vocabularies = [new() { Name = "Parasite", Members = parasites.Select(x => new VocabularyMemberDefinition { Name = char.ToUpper(x[0]) + x[1..] }).ToList() }],
        };

        var none = Score(new GlyphGrammar([], allowPartialSegmentMatches: false), documents);
        var literal = Score(GlyphGrammar.FromDefinition(literals, allowPartialSegmentMatches: false), documents);
        var templated = Score(GlyphGrammar.FromDefinition(template, allowPartialSegmentMatches: false), documents);

        Assert.Equal(1, templated.Coverage);
        Assert.Equal(1, literal.Coverage);
        Assert.True(templated.TotalBits < literal.TotalBits, $"template {templated.TotalBits:N0} vs literals {literal.TotalBits:N0}");
        Assert.True(literal.TotalBits < none.TotalBits, $"literals {literal.TotalBits:N0} vs none {none.TotalBits:N0}");
    }

    [Fact]
    public void Vocabulary_members_and_synonyms_the_corpus_never_uses_are_free()
    {
        // A vocabulary is a list of words that might match: a finished grammar culls what never did, so it costs nothing.
        string[] parasites = ["fleas", "lice"];
        var documents = Enumerable.Range(0, 20)
            .Select(i => new TestDocument(TestDocument.Unnamed, $"my dog has {parasites[i % parasites.Length]}.", []))
            .ToList();

        GrammarDefinition Template(IReadOnlyList<VocabularyMemberDefinition> members) => new()
        {
            Glyphs =
            [
                new()
                {
                    Name = "MyDogHas",
                    Nibs = [new NibDefinition.Literal("my dog has"), new NibDefinition.Property("Parasite")],
                    Properties = [new() { Name = "Parasite", Type = TypeReference.Vocabulary("Parasite") }],
                },
            ],
            Vocabularies = [new() { Name = "Parasite", Members = members }],
        };

        var used = Template([new() { Name = "Fleas", Patterns = ["fleas"] }, new() { Name = "Lice" }]);
        var padded = Template(
        [
            new() { Name = "Fleas", Patterns = ["fleas", "flea infestation"] },
            new() { Name = "Lice" },
            .. Enumerable.Range(0, 40).Select(i => new VocabularyMemberDefinition { Name = $"Unused{i}", Patterns = [$"unused parasite number {i}"] }),
        ]);

        var lean = Score(GlyphGrammar.FromDefinition(used, allowPartialSegmentMatches: false), documents);
        var full = Score(GlyphGrammar.FromDefinition(padded, allowPartialSegmentMatches: false), documents);

        Assert.Equal(lean.TotalBits, full.TotalBits, precision: 6);
        Assert.Equal(2, full.VocabularyMembersUsed["Parasite"]);
    }

    static MdlScore Score(GlyphGrammar grammar, IEnumerable<IDocument> documents) =>
        MdlScorer.Score(grammar, CorpusFixture.Process(grammar, documents));
}
