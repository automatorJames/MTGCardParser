using Glyphotype.Definitions;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary>The grammar workbench: working-definition edits, what-if scoring, and committing to C#.</summary>
[Collection(CorpusCollection.Name)]
public sealed class WorkbenchTests(CorpusFixture corpus) : IDisposable
{
    readonly string _directory = Directory.CreateTempSubdirectory("workbench-tests-").FullName;

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    GrammarWorkbench CreateWorkbench(string sourceDirectory = null) =>
        new(corpus.Grammar, corpus.ProcessedDocuments,
            new(Path.Combine(_directory, "working.json"), sourceDirectory ?? _directory, "Glyphotype.Tests.Grammar", AllowPartialClauseMatches: false));

    [Fact]
    public async Task A_fresh_workbench_matches_the_committed_grammar_without_rescoring()
    {
        var workbench = CreateWorkbench();

        Assert.Empty(workbench.Changes);
        Assert.Same(await workbench.GetCommittedScoreAsync(), await workbench.GetCurrentWorkingScoreAsync());
    }

    [Fact]
    public async Task Removing_a_glyph_is_scored_and_reverting_it_undoes_the_change()
    {
        var workbench = CreateWorkbench();
        var committed = await workbench.GetCommittedScoreAsync();

        workbench.RemoveGlyph(nameof(AnimalRests));
        await workbench.RescoreAsync();

        var change = Assert.Single(workbench.Changes);
        Assert.Equal((DefinitionKind.Glyph, nameof(AnimalRests), ChangeType.Removed), (change.Kind, change.Name, change.Change));
        Assert.True(File.Exists(workbench.WorkingDefinitionPath));

        var working = await workbench.GetCurrentWorkingScoreAsync();
        Assert.NotNull(working);
        Assert.True(working.CapturedWords < committed.CapturedWords);
        Assert.DoesNotContain(working.Glyphs, x => x.Name == nameof(AnimalRests));

        workbench.Revert(DefinitionKind.Glyph, nameof(AnimalRests));

        Assert.Empty(workbench.Changes);
        Assert.False(File.Exists(workbench.WorkingDefinitionPath));
    }

    [Fact]
    public void A_glyph_another_refers_to_cannot_be_removed()
    {
        var workbench = CreateWorkbench();

        var exception = Assert.Throws<InvalidOperationException>(() => workbench.RemoveGlyph(nameof(OnDay)));

        Assert.Contains(nameof(PersonComes), exception.Message);
        Assert.Empty(workbench.Changes);
    }

    [Fact]
    public async Task A_working_definition_that_does_not_build_reports_why()
    {
        var workbench = CreateWorkbench();
        var animalRests = workbench.WorkingDefinition.Glyphs.Single(x => x.Name == nameof(AnimalRests));

        // Two properties, but the nibs only place one: validation refuses the unplaced one.
        workbench.SetGlyph(animalRests with { Nibs = [new NibDefinition.Literal("the"), new NibDefinition.Property("Animal")] });
        await workbench.RescoreAsync();

        Assert.False(workbench.LatestWorkingScore.Succeeded);
        Assert.Contains(workbench.LatestWorkingScore.Errors, x => x.Contains(nameof(AnimalRests)) && x.Contains("Place"));
        Assert.Null(await workbench.GetCurrentWorkingScoreAsync());
    }

    [Fact]
    public async Task A_working_definition_persists_across_workbenches()
    {
        CreateWorkbench().RemoveGlyph(nameof(AnimalRests));

        var reopened = CreateWorkbench();
        await reopened.RescoreAsync();

        Assert.Equal(nameof(AnimalRests), Assert.Single(reopened.Changes).Name);
    }

    [Fact]
    public void Committing_rewrites_just_the_changed_declarations_and_the_sources_compile_to_the_working_grammar()
    {
        var sourceDirectory = Directory.CreateDirectory(Path.Combine(_directory, "Grammar")).FullName;

        foreach (var file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Grammar"), "*.cs"))
            File.Copy(file, Path.Combine(sourceDirectory, Path.GetFileName(file)));

        var workbench = CreateWorkbench(sourceDirectory);
        var working = workbench.WorkingDefinition;

        // Modified: a glyph (whose doc comment must survive), an alias with a computed property (which must be
        // carried over), and a vocabulary. Removed: a top-level glyph. Added: a new glyph, in a file of its own.
        workbench.SetGlyph(Glyph(working, nameof(AnimalRests)) with
        {
            Nibs = [new NibDefinition.Literal("the"), new NibDefinition.Property("Animal"), new NibDefinition.Alternatives(["sleeps", "naps", "dozes"]), new NibDefinition.Literal("in the"), new NibDefinition.Property("Place")],
        });
        workbench.SetGlyph(Glyph(working, nameof(ShoppingList)) with { TokenizationOrder = 5 });
        workbench.SetVocabulary(working.Vocabularies.Single(x => x.Name == nameof(Place)) is var place
            ? place with { Members = [.. place.Members, new() { Name = "Attic" }] }
            : null);
        workbench.RemoveGlyph(nameof(BakerOpensTheShop));
        workbench.SetGlyph(new GlyphDefinition
        {
            Name = "AnimalHides",
            Nibs = [new NibDefinition.Literal("the"), new NibDefinition.Property("Animal"), new NibDefinition.Literal("hides")],
            Properties = [new() { Name = "Animal", Type = TypeReference.Vocabulary(nameof(Animal)) }],
        });

        var plan = workbench.PlanCommit();
        Assert.Equal(5, plan.Declarations.Count);
        Assert.Contains(plan.Declarations, x => x.Change.Name == nameof(ShoppingList) && x.Notes.Any(y => y.Contains("NeedsEggs")));

        workbench.Commit(plan);

        Assert.Empty(workbench.Changes);

        var basicGlyphs = File.ReadAllText(Path.Combine(sourceDirectory, "BasicGlyphs.cs"));
        Assert.Contains("/// <summary>Literal nibs, <c>Alt()</c>, plain enums", basicGlyphs);
        Assert.Contains("Alt(\"sleeps\", \"naps\", \"dozes\")", basicGlyphs);
        Assert.Contains("NeedsEggs", File.ReadAllText(Path.Combine(sourceDirectory, "CollectionGlyphs.cs")));
        Assert.DoesNotContain($"class {nameof(BakerOpensTheShop)}", File.ReadAllText(Path.Combine(sourceDirectory, "LineRuleGlyphs.cs")));
        Assert.True(File.Exists(Path.Combine(sourceDirectory, "AnimalHides.cs")));

        // Each declaration written is stamped with where and when it came from - once, a later commit replacing the stamp.
        var stamp = SourceCommitter.CommitStamp(DateTime.Today);
        Assert.Contains(stamp + Environment.NewLine + "public class AnimalHides", File.ReadAllText(Path.Combine(sourceDirectory, "AnimalHides.cs")).ReplaceLineEndings());
        Assert.Contains(stamp + Environment.NewLine + "/// <summary>Literal nibs, <c>Alt()</c>, plain enums, an enum synonym, and a multi-word enum member.</summary>" + Environment.NewLine + $"public class {nameof(AnimalRests)}", basicGlyphs.ReplaceLineEndings());

        workbench.SetGlyph(Glyph(workbench.WorkingDefinition, nameof(AnimalRests)) with { TokenizationOrder = 7 });
        workbench.Commit(workbench.PlanCommit());
        basicGlyphs = File.ReadAllText(Path.Combine(sourceDirectory, "BasicGlyphs.cs"));
        Assert.Single(System.Text.RegularExpressions.Regex.Matches(basicGlyphs, System.Text.RegularExpressions.Regex.Escape(SourceCommitter.CommitStampPrefix)));
        Assert.Contains("/// <summary>Literal nibs, <c>Alt()</c>, plain enums", basicGlyphs);

        // Everything else is untouched, and what's there compiles back to exactly the working grammar.
        var compiled = SourceCompiler.Compile(Directory.GetFiles(sourceDirectory, "*.cs").Select(File.ReadAllText).ToArray());
        Assert.Empty(DefinitionDiff.Compare(workbench.WorkingDefinition, SourceCommitter.WithSummaries(GrammarDefinition.FromTypes(compiled.GetTypes()), sourceDirectory)));
    }

    [Fact]
    public void Summaries_come_from_the_sources_doc_comments_and_commit_back_to_them()
    {
        var sourceDirectory = Directory.CreateDirectory(Path.Combine(_directory, "Grammar")).FullName;

        foreach (var file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Grammar"), "*.cs"))
            File.Copy(file, Path.Combine(sourceDirectory, Path.GetFileName(file)));

        var basicGlyphsPath = Path.Combine(sourceDirectory, "BasicGlyphs.cs");
        File.WriteAllText(basicGlyphsPath, File.ReadAllText(basicGlyphsPath).Replace(
            "/// <summary><c>Opt()</c>: an optional literal.</summary>",
            "/// <summary><c>Opt()</c>: an optional literal.</summary>\n/// <remarks>Kept.</remarks>"));

        var workbench = CreateWorkbench(sourceDirectory);
        Assert.Equal("<c>Opt()</c>: an optional literal.", Glyph(workbench.CommittedDefinition, nameof(AnimalEats)).Summary);
        Assert.Empty(workbench.Changes);

        workbench.SetGlyph(Glyph(workbench.WorkingDefinition, nameof(AnimalEats)) with { TokenizationOrder = 4 });

        // A changed summary is written in place of the old one; a comment holding more than an unchanged summary is kept.
        workbench.SetGlyph(Glyph(workbench.WorkingDefinition, nameof(AnimalRests)) with { Summary = "Resting,\nover two lines." });
        workbench.Commit(workbench.PlanCommit());

        var basicGlyphs = File.ReadAllText(basicGlyphsPath).ReplaceLineEndings("\n");
        Assert.Contains("/// <summary>\n/// Resting,\n/// over two lines.\n/// </summary>\npublic class AnimalRests", basicGlyphs);
        Assert.DoesNotContain("plain enums", basicGlyphs);
        Assert.Contains("/// <summary><c>Opt()</c>: an optional literal.</summary>\n/// <remarks>Kept.</remarks>\n[TokenizationOrder(4)]", basicGlyphs);

        // The sources now hold the working summaries, so a workbench opened on them starts with nothing to commit.
        var compiled = SourceCompiler.Compile(Directory.GetFiles(sourceDirectory, "*.cs").Select(File.ReadAllText).ToArray());
        Assert.Empty(DefinitionDiff.Compare(workbench.WorkingDefinition, SourceCommitter.WithSummaries(GrammarDefinition.FromTypes(compiled.GetTypes()), sourceDirectory)));
    }

    static GlyphDefinition Glyph(GrammarDefinition grammar, string name) => grammar.Glyphs.Single(x => x.Name == name);
}
