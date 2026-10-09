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

        // A declaration is written as it is, with nothing added above it - and a later commit doesn't repeat its doc comment.
        Assert.DoesNotContain("//", File.ReadAllText(Path.Combine(sourceDirectory, "AnimalHides.cs")));

        workbench.SetGlyph(Glyph(workbench.WorkingDefinition, nameof(AnimalRests)) with { TokenizationOrder = 7 });
        workbench.Commit(workbench.PlanCommit());
        basicGlyphs = File.ReadAllText(Path.Combine(sourceDirectory, "BasicGlyphs.cs"));
        Assert.Single(System.Text.RegularExpressions.Regex.Matches(basicGlyphs, System.Text.RegularExpressions.Regex.Escape("/// <summary>Literal nibs, <c>Alt()</c>, plain enums")));
        Assert.Contains("/// <summary>Literal nibs, <c>Alt()</c>, plain enums, an enum synonym, and a multi-word enum member.</summary>" + Environment.NewLine + "[TokenizationOrder(7)]" + Environment.NewLine + $"public class {nameof(AnimalRests)}", basicGlyphs.ReplaceLineEndings());

        // Everything else is untouched, and what's there compiles back to exactly the working grammar.
        var compiled = SourceCompiler.Compile(Directory.GetFiles(sourceDirectory, "*.cs").Select(File.ReadAllText).ToArray());
        Assert.Empty(DefinitionDiff.Compare(workbench.WorkingDefinition, SourceCommitter.WithDocumentation(GrammarDefinition.FromTypes(compiled.GetTypes()), sourceDirectory)));
    }

    [Fact]
    public void Documentation_comes_from_the_sources_doc_comments_and_commits_back_to_them()
    {
        var sourceDirectory = Directory.CreateDirectory(Path.Combine(_directory, "Grammar")).FullName;

        foreach (var file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Grammar"), "*.cs"))
            File.Copy(file, Path.Combine(sourceDirectory, Path.GetFileName(file)));

        var workbench = CreateWorkbench(sourceDirectory);
        Assert.Equal("<c>Opt()</c>: an optional literal.", Glyph(workbench.CommittedDefinition, nameof(AnimalEats)).Documentation.Summary);
        Assert.Empty(workbench.Changes);

        // Changed documentation is written in place of the old; a glyph changed otherwise has its comment written as it was.
        workbench.SetGlyph(Glyph(workbench.WorkingDefinition, nameof(AnimalEats)) with { TokenizationOrder = 4 });
        workbench.SetGlyph(Glyph(workbench.WorkingDefinition, nameof(AnimalRests)) with
        {
            Documentation = new() { Summary = "Resting,\nover two lines.", ExampleDocument = "Pet diary", ExampleCapture = "the dog sleeps in the garden" },
        });
        workbench.Commit(workbench.PlanCommit());

        var basicGlyphsPath = Path.Combine(sourceDirectory, "BasicGlyphs.cs");
        var basicGlyphs = File.ReadAllText(basicGlyphsPath).ReplaceLineEndings("\n");
        Assert.Contains("/// <summary>\n/// Resting,\n/// over two lines.\n/// </summary>\n/// <exampledoc>Pet diary</exampledoc>\n/// <examplecapture>the dog sleeps in the garden</examplecapture>\npublic class AnimalRests", basicGlyphs);
        Assert.DoesNotContain("plain enums", basicGlyphs);
        Assert.Contains("/// <summary><c>Opt()</c>: an optional literal.</summary>\n[TokenizationOrder(4)]", basicGlyphs);

        // The sources now hold the working documentation, so a workbench opened on them starts with nothing to commit.
        var compiled = SourceCompiler.Compile(Directory.GetFiles(sourceDirectory, "*.cs").Select(File.ReadAllText).ToArray());
        Assert.Empty(DefinitionDiff.Compare(workbench.WorkingDefinition, SourceCommitter.WithDocumentation(GrammarDefinition.FromTypes(compiled.GetTypes()), sourceDirectory)));

        // Whatever else a doc comment holds is no part of the documentation, and a commit keeps it, after the documentation.
        File.WriteAllText(basicGlyphsPath, basicGlyphs.Replace("/// <summary><c>Opt()</c>", "/// Eats, sometimes.\n/// <remarks>Kept.</remarks>\n/// <summary><c>Opt()</c>"));
        workbench = CreateWorkbench(sourceDirectory);
        Assert.Equal(new GlyphDocumentation { Summary = "<c>Opt()</c>: an optional literal." }, Glyph(workbench.CommittedDefinition, nameof(AnimalEats)).Documentation);

        workbench.SetGlyph(Glyph(workbench.WorkingDefinition, nameof(AnimalEats)) with { TokenizationOrder = 5 });
        workbench.Commit(workbench.PlanCommit());
        Assert.Contains("/// <summary><c>Opt()</c>: an optional literal.</summary>\n/// Eats, sometimes.\n/// <remarks>Kept.</remarks>\n[TokenizationOrder(5)]",
            File.ReadAllText(basicGlyphsPath).ReplaceLineEndings("\n"));
    }

    /// <summary>
    /// A project laid out as a real one is: a <c>.csproj</c>, the glyph sources in a directory of their own, and the
    /// vocabularies - plus an enum no glyph uses - in a file beside it rather than in it.
    /// </summary>
    string CreateProject()
    {
        var projectDirectory = Directory.CreateDirectory(Path.Combine(_directory, "Project")).FullName;
        var sourceDirectory = Directory.CreateDirectory(Path.Combine(projectDirectory, "Grammar")).FullName;
        File.WriteAllText(Path.Combine(projectDirectory, "Project.csproj"), "<Project />");

        foreach (var file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Grammar"), "*.cs"))
        {
            var vocabulary = Path.GetFileName(file) == "Vocabulary.cs";
            File.Copy(file, Path.Combine(vocabulary ? projectDirectory : sourceDirectory, Path.GetFileName(file)));
        }

        File.WriteAllText(Path.Combine(projectDirectory, "Unused.cs"), "namespace Glyphotype.Tests.Grammar;\n\npublic enum Mood\n{\n    Calm,\n}\n");
        return sourceDirectory;
    }

    static string[] ProjectSources(string sourceDirectory) =>
        Directory.GetFiles(Path.GetDirectoryName(sourceDirectory), "*.cs", SearchOption.AllDirectories).Select(File.ReadAllText).ToArray();

    [Fact]
    public void Committing_rewrites_a_vocabulary_where_the_project_declares_it_and_puts_a_new_one_with_the_other_enums()
    {
        var sourceDirectory = CreateProject();
        var workbench = CreateWorkbench(sourceDirectory);
        var place = workbench.WorkingDefinition.Vocabularies.Single(x => x.Name == nameof(Place));

        workbench.SetVocabulary(place with { Members = [.. place.Members, new() { Name = "Attic" }] });
        workbench.SetVocabulary(new VocabularyDefinition { Name = "Season", Members = [new() { Name = "Spring" }] });
        workbench.SetGlyph(new GlyphDefinition
        {
            Name = "ItIsSeason",
            Nibs = [new NibDefinition.Literal("it is"), new NibDefinition.Property("Season")],
            Properties = [new() { Name = "Season", Type = TypeReference.Vocabulary("Season") }],
        });

        var plan = workbench.PlanCommit();
        Assert.Empty(plan.Conflicts);
        workbench.Commit(plan);

        var projectDirectory = Path.GetDirectoryName(sourceDirectory);
        Assert.Contains("Attic", File.ReadAllText(Path.Combine(projectDirectory, "Vocabulary.cs")));
        Assert.False(File.Exists(Path.Combine(sourceDirectory, "Place.cs")));
        Assert.True(File.Exists(Path.Combine(sourceDirectory, SourceCommitter.VocabularyDirectory, "Season.cs")));
        Assert.True(File.Exists(Path.Combine(sourceDirectory, "ItIsSeason.cs")));

        var compiled = SourceCompiler.Compile(ProjectSources(sourceDirectory));
        Assert.Empty(DefinitionDiff.Compare(workbench.WorkingDefinition, SourceCommitter.WithDocumentation(GrammarDefinition.FromTypes(compiled.GetTypes()), sourceDirectory)));
    }

    [Theory]
    [InlineData(NameConflictResolution.RenameExisting)]
    [InlineData(NameConflictResolution.RenameIncoming)]
    public void A_new_definition_named_like_a_type_in_the_project_waits_for_the_person_to_say_which_to_rename(NameConflictResolution resolution)
    {
        var sourceDirectory = CreateProject();
        var workbench = CreateWorkbench(sourceDirectory);

        workbench.SetVocabulary(new VocabularyDefinition { Name = "Mood", Members = [new() { Name = "Happy" }, new() { Name = "Sad" }] });
        workbench.SetGlyph(new GlyphDefinition
        {
            Name = "AnimalFeels",
            Nibs = [new NibDefinition.Literal("the"), new NibDefinition.Property("Animal"), new NibDefinition.Literal("feels"), new NibDefinition.Property("Mood")],
            Properties = [new() { Name = "Animal", Type = TypeReference.Vocabulary(nameof(Animal)) }, new() { Name = "Mood", Type = TypeReference.Vocabulary("Mood") }],
        });

        var unresolved = workbench.PlanCommit();
        Assert.Equal("Mood", Assert.Single(unresolved.Conflicts).Change.Name);
        Assert.False(unresolved.IsResolved);
        Assert.Throws<InvalidOperationException>(() => workbench.Commit(unresolved));

        workbench.Commit(workbench.PlanCommit(resolution));

        var enums = Path.Combine(sourceDirectory, SourceCommitter.VocabularyDirectory);
        var unused = File.ReadAllText(Path.Combine(Path.GetDirectoryName(sourceDirectory), "Unused.cs"));

        if (resolution == NameConflictResolution.RenameExisting)
        {
            Assert.Contains("enum Mood_legacy", unused);
            Assert.True(File.Exists(Path.Combine(enums, "Mood.cs")));
        }
        else
        {
            Assert.Contains("enum Mood\n", unused);
            Assert.True(File.Exists(Path.Combine(enums, "Mood_new.cs")));
            Assert.Contains(workbench.WorkingDefinition.Glyphs.Single(x => x.Name == "AnimalFeels").Properties, x => x.Type.Name == "Mood_new");
        }

        Assert.Empty(workbench.Changes);

        var compiled = SourceCompiler.Compile(ProjectSources(sourceDirectory));
        Assert.Empty(DefinitionDiff.Compare(workbench.WorkingDefinition, SourceCommitter.WithDocumentation(GrammarDefinition.FromTypes(compiled.GetTypes()), sourceDirectory)));
    }

    static GlyphDefinition Glyph(GrammarDefinition grammar, string name) => grammar.Glyphs.Single(x => x.Name == name);
}
