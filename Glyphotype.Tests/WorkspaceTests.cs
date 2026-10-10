using System.IO.Compression;
using Glyphotype.Definitions;
using Glyphotype.Distiller.Agent;
using Glyphotype.Distiller.Workbench;
using Glyphotype.Distiller.Workspaces;

namespace Glyphotype.Tests;

/// <summary>Workspaces: grammars kept apart from the C# sources, each with its own baseline, working changes and history.</summary>
[Collection(CorpusCollection.Name)]
public sealed class WorkspaceTests(CorpusFixture corpus) : IDisposable
{
    const string _animalSnores = """
        public class AnimalSnores : Glyph
        {
            public override Nib[] Nibs => ["the", Prop(Animal), "snores in the", Prop(Place)];

            public Animal Animal { get; set; }
            public Place Place { get; set; }
        }
        """;

    readonly string _root = Directory.CreateTempSubdirectory("workspace-tests-").FullName;

    public void Dispose() => Directory.Delete(_root, recursive: true);

    WorkspaceManager CreateManager(string legacyWorkingDefinitionPath = null) =>
        new(new SourceWorkspace("TestGrammar", corpus.Grammar, corpus.ProcessedDocuments, Path.Combine(_root, "sources"), "Glyphotype.Tests.Grammar"),
            Path.Combine(_root, "workspaces"), allowPartialClauseMatches: false, legacyWorkingDefinitionPath);

    static ChangeSet Changes(GrammarWorkbench workbench, string source) =>
        ChangeSet.FromSource(source, [], workbench.WorkingDefinition);

    [Fact]
    public void A_new_manager_offers_the_source_grammar_alone()
    {
        var workspaces = CreateManager();

        var source = Assert.Single(workspaces.Workspaces);
        Assert.Equal(("TestGrammar", WorkspaceKind.Source), (source.Name, source.Kind));
        Assert.Equal(source, workspaces.ActiveWorkspace);
        Assert.True(workspaces.Active.IsSourceBacked);
    }

    [Fact]
    public async Task Starting_from_scratch_keeps_the_vocabularies_and_nothing_else()
    {
        var workspaces = CreateManager();
        var source = workspaces.Active;

        var scratch = workspaces.Create("From scratch", WorkspaceSeed.Vocabularies);

        Assert.Equal(scratch, workspaces.ActiveWorkspace);
        Assert.False(workspaces.Active.IsSourceBacked);
        Assert.Empty(workspaces.Active.WorkingDefinition.Glyphs);
        Assert.Equal(source.WorkingDefinition.Vocabularies.Select(x => x.Name), workspaces.Active.WorkingDefinition.Vocabularies.Select(x => x.Name));
        Assert.Empty(workspaces.Active.Changes);

        // Unused vocabularies cost nothing: it's the no-grammar baseline.
        var score = await workspaces.Active.GetCommittedScoreAsync();
        Assert.Equal(score.BaselineBits, score.TotalBits, precision: 6);

        // And the source grammar is untouched.
        Assert.Empty(source.Changes);
    }

    [Fact]
    public async Task A_scratch_workspace_checkpoints_instead_of_committing()
    {
        var workspaces = CreateManager();
        workspaces.Create("Scratch", WorkspaceSeed.Vocabularies);
        var workbench = workspaces.Active;

        Assert.Throws<InvalidOperationException>(() => workbench.PlanCommit());

        workbench.Apply(Changes(workbench, _animalSnores));
        var trial = await workbench.GetCurrentTrialAsync();
        workbench.Checkpoint();

        Assert.Empty(workbench.Changes);
        Assert.Empty(workbench.History);
        Assert.Contains(workbench.CommittedDefinition.Glyphs, x => x.Name == "AnimalSnores");
        Assert.Same(trial.Score, await workbench.GetCommittedScoreAsync());
    }

    [Fact]
    public async Task Workspaces_their_changes_and_their_history_survive_a_restart()
    {
        var workspaces = CreateManager();
        workspaces.Create("Scratch", WorkspaceSeed.Vocabularies);
        workspaces.Active.Apply(Changes(workspaces.Active, _animalSnores), "snoring animals");
        await workspaces.Active.GetCurrentTrialAsync();
        workspaces.Active.Dispose();

        var restarted = CreateManager();

        Assert.Equal(["TestGrammar", "Scratch"], restarted.Workspaces.Select(x => x.Name));
        Assert.Equal("Scratch", restarted.ActiveWorkspace.Name);
        Assert.Contains(restarted.Active.WorkingDefinition.Glyphs, x => x.Name == "AnimalSnores");

        var step = Assert.Single(restarted.Active.History);
        Assert.Equal("snoring animals", step.Description);

        restarted.Active.Undo();
        Assert.Empty(restarted.Active.WorkingDefinition.Glyphs);
        Assert.Empty(restarted.Active.Changes);
    }

    [Fact]
    public void An_export_compiles_to_the_grammar_it_came_from()
    {
        var workspaces = CreateManager();
        workspaces.Create("A copy", WorkspaceSeed.Copy);
        workspaces.Active.Apply(Changes(workspaces.Active, _animalSnores));

        using var zip = new ZipArchive(new MemoryStream(workspaces.Export("A copy", "Exported.Grammar")));

        Assert.Contains(zip.Entries, x => x.Name == "AnimalSnores.cs");
        Assert.Contains(zip.Entries, x => x.Name == "grammar.json");

        var sources = zip.Entries.Where(x => x.Name.EndsWith(".cs")).Select(x => new StreamReader(x.Open()).ReadToEnd()).ToArray();
        Assert.All(sources, x => Assert.StartsWith("namespace Exported.Grammar;", x));

        var compiled = GrammarDefinition.FromTypes(SourceCompiler.Compile(sources).GetTypes());
        Assert.Empty(DefinitionDiff.Compare(workspaces.Active.WorkingDefinition.WithoutUnreferencedTerminals(), compiled));
    }

    [Fact]
    public void Scratch_workspaces_can_be_renamed_and_deleted_but_the_source_one_cant()
    {
        var workspaces = CreateManager();
        var folder = Path.Combine(_root, "workspaces", workspaces.Create("Scratch").Folder);

        workspaces.Rename("Scratch", "Experiment");
        Assert.Equal("Experiment", workspaces.ActiveWorkspace.Name);
        Assert.Throws<InvalidOperationException>(() => workspaces.Create("experiment"));

        workspaces.Delete("Experiment");
        Assert.Equal(WorkspaceKind.Source, workspaces.ActiveWorkspace.Kind);
        Assert.False(Directory.Exists(folder));

        Assert.Throws<InvalidOperationException>(() => workspaces.Delete("TestGrammar"));
        Assert.Throws<InvalidOperationException>(() => workspaces.Rename("TestGrammar", "Other"));
    }

    [Fact]
    public void The_source_grammars_working_changes_from_before_workspaces_move_into_its_workspace()
    {
        var definition = corpus.Grammar.ToDefinition().WithoutGlyph("AnimalRests");
        var legacy = Path.Combine(_root, "working-grammar.json");
        File.WriteAllText(legacy, definition.ToJson());

        var workspaces = CreateManager(legacy);

        Assert.False(File.Exists(legacy));
        Assert.Equal("AnimalRests", Assert.Single(workspaces.Active.Changes).Name);
    }

    [Fact]
    public async Task Guidance_is_kept_per_workspace_copied_on_request_and_given_to_the_agent()
    {
        var workspaces = CreateManager();
        Assert.Equal("", workspaces.GetGuidance());

        workspaces.SetGuidance("  Rules text is terse.  ");
        Assert.Equal("Rules text is terse.", workspaces.GetGuidance());

        workspaces.Create("Plain", WorkspaceSeed.Vocabularies);
        Assert.Equal("", workspaces.GetGuidance());

        workspaces.Create("Informed", WorkspaceSeed.Vocabularies, "TestGrammar", copyGuidance: true);
        Assert.Equal("Rules text is terse.", workspaces.GetGuidance());
        Assert.Equal("Rules text is terse.", CreateManager().GetGuidance("Informed"));

        var brief = await new GrammarAgent(workspaces, "the test corpus", AgentTests.AnySteps).StartSessionAsync();
        Assert.Contains("Rules text is terse.", brief);

        workspaces.SetGuidance("");
        Assert.Equal("", workspaces.GetGuidance());
        Assert.Equal("Rules text is terse.", workspaces.GetGuidance("TestGrammar"));
    }

    [Fact]
    public void Configured_guidance_overrides_the_workspace_folders_and_cant_be_set_here()
    {
        CreateManager().SetGuidance("Kept on this machine.");

        var workspaces = new WorkspaceManager(
            new SourceWorkspace("TestGrammar", corpus.Grammar, corpus.ProcessedDocuments, Path.Combine(_root, "sources"), "Glyphotype.Tests.Grammar"),
            Path.Combine(_root, "workspaces"), allowPartialClauseMatches: false,
            configuredGuidance: new Dictionary<string, string> { ["TestGrammar"] = " Travels with the code. " });

        Assert.True(workspaces.IsGuidanceConfigured());
        Assert.Equal("Travels with the code.", workspaces.GetGuidance());
        Assert.Throws<InvalidOperationException>(() => workspaces.SetGuidance("Changed."));

        workspaces.Create("Informed", WorkspaceSeed.Vocabularies, copyGuidance: true);
        Assert.False(workspaces.IsGuidanceConfigured());
        Assert.Equal("Travels with the code.", workspaces.GetGuidance());
    }

    [Fact]
    public async Task Merging_a_copy_brings_what_it_changed_into_the_workspace_it_came_from_and_leaves_it_clean()
    {
        var workspaces = CreateManager();
        workspaces.Create("Branch", WorkspaceSeed.Copy);
        var branch = workspaces.Active;
        branch.Apply(Changes(branch, _animalSnores));
        await branch.GetCurrentTrialAsync();
        branch.Checkpoint();
        branch.RemoveGlyph("AnimalRests");
        workspaces.GetJournal().Add(JournalSection.Hint, "Snoring is its own glyph.", ["AnimalSnores"], branch.WorkingDefinition);

        Assert.Equal("TestGrammar", workspaces.DefaultMergeTarget("Branch"));
        var merge = workspaces.PlanMerge("Branch", "TestGrammar");

        Assert.True(merge.IsBaseRecorded);
        Assert.Equal([("AnimalSnores", ChangeType.Added), ("AnimalRests", ChangeType.Removed)], merge.Items.Select(x => (x.Name, x.Change)));
        Assert.DoesNotContain(merge.Items, x => x.IsConflict);

        workspaces.Merge(merge, merge.CleanItems);

        Assert.Equal("TestGrammar", workspaces.ActiveWorkspace.Name);
        Assert.Equal(["AnimalRests", "AnimalSnores"], workspaces.Active.Changes.Select(x => x.Name).Order());
        Assert.Equal("merge Branch", Assert.Single(workspaces.Active.History).Description);

        // The branch has nothing left to merge, nor any working changes, so could go without losing anything.
        Assert.Empty(workspaces.PlanMerge("Branch", "TestGrammar").Items);
        workspaces.Switch("Branch");
        Assert.Empty(workspaces.Active.Changes);
        Assert.Empty(workspaces.Active.History);
        Assert.Contains(workspaces.Active.CommittedDefinition.Glyphs, x => x.Name == "AnimalSnores");
        Assert.Equal("Merged into TestGrammar", workspaces.GetJournal().Versions[0].Label);

        // Kept iterating, the branch's next merge offers only what changed since.
        workspaces.Active.Apply(Changes(workspaces.Active, _animalSnores.Replace("snores in the", "snores loudly in the")));
        var next = Assert.Single(workspaces.PlanMerge("Branch", "TestGrammar").Items);
        Assert.Equal(("AnimalSnores", ChangeType.Modified, false), (next.Name, next.Change, next.IsConflict));
    }

    [Fact]
    public void A_definition_both_workspaces_changed_is_a_conflict_and_what_isnt_taken_is_offered_again()
    {
        var workspaces = CreateManager();
        workspaces.Active.Apply(Changes(workspaces.Active, _animalSnores.Replace("snores in the", "snores softly in the")));
        workspaces.Create("Branch", WorkspaceSeed.Copy);
        workspaces.Switch("TestGrammar");
        workspaces.Active.Apply(Changes(workspaces.Active, _animalSnores.Replace("snores in the", "snores loudly in the")));
        workspaces.Switch("Branch");
        workspaces.Active.Apply(Changes(workspaces.Active, _animalSnores));

        var merge = workspaces.PlanMerge("Branch", "TestGrammar");
        var conflict = Assert.Single(merge.Items);
        Assert.True(conflict.IsConflict);
        Assert.Empty(merge.CleanItems);

        workspaces.Merge(merge, merge.CleanItems);

        Assert.Contains(workspaces.Active.WorkingDefinition.Glyphs, x => x.Name == "AnimalSnores" && DefinitionJson.Serialize(x).Contains("loudly"));
        Assert.Equal("AnimalSnores", Assert.Single(workspaces.PlanMerge("Branch", "TestGrammar").Items).Name);
    }

    [Fact]
    public void A_merge_cant_leave_a_glyph_referring_to_what_it_removed()
    {
        var workspaces = CreateManager();
        workspaces.Create("Branch", WorkspaceSeed.Copy);
        var definition = workspaces.Active.WorkingDefinition;

        // A glyph referred to by one other glyph alone, which nothing refers to.
        var (referrer, referenced) = definition.Glyphs
            .Where(x => definition.GetReferrers(x.Name).Count == 1)
            .Select(x => (Referrer: definition.GetReferrers(x.Name)[0], Referenced: x.Name))
            .First(x => definition.GetReferrers(x.Referrer).Count == 0);

        workspaces.Active.Restore(definition.WithoutGlyph(referrer).WithoutGlyph(referenced), "remove both");

        var merge = workspaces.PlanMerge("Branch", "TestGrammar");

        Assert.Throws<InvalidOperationException>(() => workspaces.Merge(merge, merge.Items.Where(x => x.Name == referenced)));
        Assert.Equal("Branch", workspaces.ActiveWorkspace.Name);

        workspaces.Merge(merge, merge.Items);
        Assert.Equal(2, workspaces.Active.Changes.Count);
    }

    [Fact]
    public void A_workspace_started_before_merge_bases_were_kept_merges_from_its_baseline_if_never_checkpointed()
    {
        var workspaces = CreateManager();
        var folder = Path.Combine(_root, "workspaces", workspaces.Create("Branch", WorkspaceSeed.Copy).Folder);
        File.Delete(Path.Combine(folder, "merge-base.json"));
        workspaces.Active.Apply(Changes(workspaces.Active, _animalSnores));

        var merge = workspaces.PlanMerge("Branch", "TestGrammar");
        Assert.True(merge.IsBaseRecorded);
        Assert.Equal("AnimalSnores", Assert.Single(merge.Items).Name);

        // A baseline written well after the workspace started was checkpointed: what it started from is lost.
        File.SetLastWriteTimeUtc(Path.Combine(folder, "baseline.json"), DateTime.UtcNow.AddHours(1));
        Assert.False(workspaces.PlanMerge("Branch", "TestGrammar").IsBaseRecorded);
    }

    [Fact]
    public async Task An_agent_works_on_the_active_workspace_and_can_start_one_from_scratch()
    {
        var workspaces = CreateManager();
        var agent = new GrammarAgent(workspaces, "the test corpus", AgentTests.AnySteps);

        Assert.Contains("Workspace: TestGrammar (source", await agent.OverviewAsync());

        var created = await agent.CreateWorkspaceAsync("Fresh", start: "vocabularies");
        Assert.Contains("Created and switched to Fresh (scratch", created);
        Assert.Contains("Working grammar: 0 glyphs", created);

        await agent.ApplyAsync(_animalSnores);
        Assert.Contains(workspaces.Active.WorkingDefinition.Glyphs, x => x.Name == "AnimalSnores");
        Assert.Contains("* Fresh", agent.ListWorkspaces());

        await agent.SwitchWorkspaceAsync("TestGrammar");
        Assert.DoesNotContain(workspaces.Active.WorkingDefinition.Glyphs, x => x.Name == "AnimalSnores");
    }
}
