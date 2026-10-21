using Glyphotype.Definitions;
using Glyphotype.Distiller.Workbench;
using Glyphotype.Tokenizers;

namespace Glyphotype.Tests;

/// <summary>
/// Re-scoring after an edit tokenizes again only the lines the edit could change (see <see cref="TokenizationChange"/>) -
/// and gets exactly what tokenizing the whole corpus would.
/// </summary>
[Collection(CorpusCollection.Name)]
public sealed class IncrementalTokenizationTests : IDisposable
{
    readonly CorpusFixture _corpus;
    readonly string _directory = Directory.CreateTempSubdirectory("incremental-tests-").FullName;

    public IncrementalTokenizationTests(CorpusFixture corpus)
    {
        _corpus = corpus;

        // As a workspace manager does for the compiled grammar: rebuilt glyphs that haven't changed are the same types.
        GrammarEmitter.Reuse(corpus.Grammar.Types);
    }

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    [Fact]
    public void A_grammar_tokenizes_nothing_differently_from_itself()
    {
        var change = TokenizationChange.Between(_corpus.Grammar, _corpus.Grammar);

        Assert.NotNull(change);
        Assert.True(change.IsNone);
    }

    [Fact]
    public async Task Each_edit_is_tokenized_as_the_whole_corpus_would_be()
    {
        var workbench = new GrammarWorkbench(_corpus.Grammar, _corpus.ProcessedDocuments,
            new(Path.Combine(_directory, "working.json"), _directory, "Glyphotype.Tests.Grammar"));

        var reusedAny = false;
        var before = _corpus.ProcessedDocuments;

        async Task AssertTokenizedAsWhole(string edit)
        {
            await workbench.RescoreAsync();
            var trial = await workbench.GetCurrentTrialAsync();
            Assert.True(trial.Succeeded, $"{edit}: {string.Join("; ", trial.Errors)}");

            var whole = CorpusFixture.Process(trial.Grammar, _corpus.ProcessedDocuments.Select(x => x.Document));

            Assert.Equal(Signatures(whole), Signatures(trial.Documents));
            Assert.DoesNotContain(trial.Documents, x => x.IsSupersededBy(trial.Grammar));

            reusedAny |= trial.Documents.SelectMany(x => x.Lines).Intersect(before.SelectMany(x => x.Lines)).Any();
            before = trial.Documents;
        }

        workbench.RemoveGlyph(nameof(AnimalRests));
        await AssertTokenizedAsWhole("remove a glyph");

        workbench.SetGlyph(Read("""
            public class AnimalSnores : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal), "snores"];

                public Animal Animal { get; set; }
            }
            """));
        await AssertTokenizedAsWhole("add a glyph");

        var animal = workbench.WorkingDefinition.Vocabularies.Single(x => x.Name == nameof(Animal));
        workbench.SetVocabulary(animal with { Members = [.. animal.Members, new() { Name = "Goat" }] });
        await AssertTokenizedAsWhole("add a vocabulary member");

        // Inlined in KnocksOnTheDoor, with a pattern whose matching depends on more than the text it matches.
        workbench.SetGlyph(Read("""
            [Dependent]
            public class OurNeighbor : Glyph
            {
                public override Nib[] Nibs => ["our", Pattern("(?=neighbou?r)[a-z]+")];
            }
            """));
        await AssertTokenizedAsWhole("edit an inlined glyph to look ahead");

        // Reached only through dynamics, so it changes lines whose result never names it.
        workbench.SetGlyph(Read("""
            [Dependent]
            public class WeFeedTheBirds : Glyph, IChore
            {
                public override Nib[] Nibs => ["we feed the birds"];
            }
            """));
        await AssertTokenizedAsWhole("add a glyph only a dynamic resolves to");

        workbench.RemoveGlyph(nameof(CatDozesInTheSun));
        await AssertTokenizedAsWhole("remove a glyph a dynamic resolves to");

        // A pattern that looks around itself, so whether it matches depends on more than the text it matches.
        workbench.SetGlyph(Read("""
            public class AnimalBeforeSleeping : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal), Pattern("(?=sleeps)[a-z]+"), "in the", Prop(Place)];

                public Animal Animal { get; set; }
                public Place Place { get; set; }
            }
            """));
        await AssertTokenizedAsWhole("add a glyph whose pattern looks ahead");

        workbench.Undo();
        workbench.Undo();
        await AssertTokenizedAsWhole("undo");

        Assert.True(reusedAny, "No edit reused a line - every one tokenized the whole corpus");
    }

    [Fact]
    public async Task A_line_none_of_an_edited_glyphs_text_is_in_is_kept_though_the_glyph_inlining_it_is_rebuilt()
    {
        var workbench = new GrammarWorkbench(_corpus.Grammar, _corpus.ProcessedDocuments,
            new(Path.Combine(_directory, "working.json"), _directory, "Glyphotype.Tests.Grammar"));

        // KnocksOnTheDoor inlines OurNeighbor, so is rebuilt with it - but differs only inside OurNeighbor's group.
        var changes = ChangeSet.FromSource("""
            [Dependent]
            public class OurNeighbor : Glyph
            {
                public override Nib[] Nibs => ["our", Alt("neighbor", "neighbour")];
            }
            """, [], workbench.WorkingDefinition);

        var evaluation = await workbench.EvaluateAsync(changes);
        Assert.True(evaluation.After.Succeeded, string.Join("; ", evaluation.After.Errors));

        var grammar = evaluation.After.Grammar;
        var whole = CorpusFixture.Process(grammar, _corpus.ProcessedDocuments.Select(x => x.Document));
        Assert.Equal(Signatures(whole), Signatures(evaluation.After.Documents));

        var before = Line(evaluation.Before.Documents, "the horse knocks on the door.");
        var after = Line(evaluation.After.Documents, "the horse knocks on the door.");
        Assert.Same(before, after);
        Assert.True(TokenizationChange.Supersedes(grammar, after), "The kept line should hold the KnocksOnTheDoor type the edit rebuilt");

        Assert.NotSame(Line(evaluation.Before.Documents, "our neighbour knocks on the door."), Line(evaluation.After.Documents, "our neighbour knocks on the door."));

        // What the workbench keeps as its state is tokenized by its own grammar's types throughout - to the same result.
        workbench.Apply(changes);
        await workbench.RescoreAsync();
        var trial = await workbench.GetCurrentTrialAsync();

        Assert.Equal(Signatures(whole), Signatures(trial.Documents));
        Assert.DoesNotContain(trial.Documents, x => x.IsSupersededBy(trial.Grammar));
    }

    static ProcessedLine Line(IEnumerable<ProcessedDocument> documents, string text) =>
        Assert.Single(documents.SelectMany(x => x.Lines), x => x.SourceText.FormattedText == text);

    GlyphDefinition Read(string source) =>
        Assert.Single(GlyphSourceReader.Read(source, _corpus.Grammar.ToDefinition()).Glyphs);

    static List<string> Signatures(IEnumerable<ProcessedDocument> documents) =>
        documents.SelectMany(x => x.Lines.Select(line => $"{x.Document.Name}#{line.SourceText.LineIndex}: {GlyphSignature.Of(line)}")).ToList();
}
