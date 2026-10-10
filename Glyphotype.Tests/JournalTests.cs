using Glyphotype.Distiller.Agent;
using Glyphotype.Distiller.Workbench;
using Glyphotype.Distiller.Workspaces;

namespace Glyphotype.Tests;

/// <summary>A workspace's journal: what agents learn, kept for every later session - and the handoff that relies on it.</summary>
[Collection(CorpusCollection.Name)]
public sealed class JournalTests(CorpusFixture corpus) : IDisposable
{
    const string _animalSnores = """
        public class AnimalSnores : Glyph
        {
            public override Nib[] Nibs => ["the", Prop(Animal), "snores in the", Prop(Place)];

            public Animal Animal { get; set; }
            public Place Place { get; set; }
        }
        """;

    const string _animalYawns = """
        public class AnimalYawns : Glyph
        {
            public override Nib[] Nibs => ["the", Prop(Animal), "yawns in the", Prop(Place)];

            public Animal Animal { get; set; }
            public Place Place { get; set; }
        }
        """;

    readonly string _directory = Directory.CreateTempSubdirectory("journal-tests-").FullName;

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    /// <summary>An agent keeping a journal, over a corpus where snoring and yawning recur.</summary>
    (GrammarAgent Agent, GrammarWorkbench Workbench, WorkspaceJournal Journal) CreateAgent(AgentSessionSettings settings = null)
    {
        string[] animals = ["dog", "cat", "bird"];
        string[] places = ["kitchen", "garden", "barn"];
        var documents = Enumerable.Range(0, 30)
            .Select(i => new TestDocument(TestDocument.Unnamed, $"the {animals[i % 3]} snores in the {places[i / 3 % 3]}.\nthe {animals[i % 3]} yawns in the {places[i / 3 % 3]}.", []));

        var workbench = new GrammarWorkbench(corpus.Grammar, CorpusFixture.Process(corpus.Grammar, documents),
            new(Path.Combine(_directory, "working.json"), _directory, "Glyphotype.Tests.Grammar", AllowPartialClauseMatches: false));

        var journal = new WorkspaceJournal(Path.Combine(_directory, "journal"));
        return (new GrammarAgent(workbench, "a snoring corpus", settings ?? AgentTests.AnySteps, journal), workbench, journal);
    }

    [Fact]
    public async Task What_one_session_records_the_next_is_briefed_with_and_a_change_to_what_it_names_flags_it()
    {
        var (agent, workbench, journal) = CreateAgent();

        await agent.StartSessionAsync();
        agent.JournalAdd("dead_end", "Matching snoring and yawning in one glyph costs more than two glyphs.", "AnimalSnores");
        agent.JournalAdd("hint", "Every line names a place.");

        var brief = await agent.StartSessionAsync();
        Assert.Contains("#1 [AnimalSnores] Matching snoring and yawning", brief);
        Assert.Contains("#2 Every line names a place.", brief);
        Assert.DoesNotContain("(may be stale", brief);

        // A step changing what an entry names is reminded of it, and the next brief flags it.
        var applied = await agent.ApplyAsync(_animalSnores);
        Assert.Contains("Journal: #1 is about what this step changed (AnimalSnores)", applied);
        Assert.Contains("#1 [AnimalSnores] (may be stale", await agent.StartSessionAsync());

        // Rewriting it fingerprints it afresh.
        agent.JournalUpdate(1, text: "Snoring has a glyph of its own now; yawning still needs one.");
        Assert.False(WorkspaceJournal.IsStale(journal.Entries.Single(x => x.Id == 1), workbench.WorkingDefinition));

        agent.JournalRemove(2);
        Assert.Equal([1], journal.Entries.Select(x => x.Id));
    }

    [Fact]
    public async Task The_journal_stays_under_its_word_limit_and_reminds_a_stuck_agent_to_record_dead_ends()
    {
        var (agent, _, _) = CreateAgent(AgentTests.AnySteps with { JournalWordLimit = 12 });
        await agent.StartSessionAsync();

        Assert.DoesNotContain("record it (`journal_add`)", await agent.EvaluateAsync(_animalYawns));
        Assert.DoesNotContain("record it (`journal_add`)", await agent.EvaluateAsync(_animalYawns));
        Assert.Contains("record it (`journal_add`)", await agent.EvaluateAsync(_animalYawns));

        // Once something's written down, it isn't asked for again until the next step.
        agent.JournalAdd("open_problem", "Yawning lines have no glyph yet.");
        await agent.EvaluateAsync(_animalYawns);
        await agent.EvaluateAsync(_animalYawns);
        Assert.DoesNotContain("record it (`journal_add`)", await agent.EvaluateAsync(_animalYawns));

        var refused = Assert.Throws<AgentRequestException>(() => agent.JournalAdd("hint", "This entry is long enough to take the journal past its limit."));
        Assert.Contains("past its limit of 12", refused.Message);
    }

    [Fact]
    public async Task A_handoff_stops_steps_until_a_fresh_session_continues_the_same_round()
    {
        var (agent, workbench, _) = CreateAgent(AgentTests.AnySteps with { StepsBeforeCheckIn = 5 });

        await agent.StartSessionAsync();
        await agent.ApplyAsync(_animalSnores);

        agent.RequestHandoff();
        Assert.Contains("Handoff due", await agent.EvaluateAsync(_animalYawns));
        Assert.Contains("Handoff due", Assert.Throws<AgentRequestException>(() => agent.ApplyAsync(_animalYawns).GetAwaiter().GetResult()).Message);

        agent.ContinueRoundInNextSession();
        var brief = await agent.StartSessionAsync();
        Assert.Contains("continuing the round", brief);
        Assert.Contains("1 of the round's 5 steps are taken", brief);

        await agent.ApplyAsync(_animalYawns);
        Assert.Equal([1, 1], workbench.History.Select(x => x.Round));

        // A session started as usual begins a round of its own.
        await agent.StartSessionAsync();
        await agent.ApplyAsync(_animalSnores.Replace("snores in the", "snores loudly in the"));
        Assert.Equal(2, workbench.History[^1].Round);
    }

    [Fact]
    public void A_workspace_keeps_a_version_of_its_journal_at_each_checkpoint_and_a_new_one_can_start_with_it()
    {
        var workspaces = new WorkspaceManager(
            new SourceWorkspace("TestGrammar", corpus.Grammar, corpus.ProcessedDocuments, Path.Combine(_directory, "sources"), "Glyphotype.Tests.Grammar"),
            Path.Combine(_directory, "workspaces"), allowPartialClauseMatches: false);

        workspaces.Create("Scratch", WorkspaceSeed.Vocabularies);
        var journal = workspaces.GetJournal();
        journal.Add(JournalSection.Hint, "Every line names a place.", [], workspaces.Active.WorkingDefinition);

        workspaces.Active.Checkpoint();
        workspaces.Active.Checkpoint();
        Assert.Equal("Checkpoint", Assert.Single(journal.Versions).Label);

        journal.Add(JournalSection.OpenProblem, "Yawning has no glyph.", [], workspaces.Active.WorkingDefinition);
        Assert.Single(journal.ReadVersion(journal.Versions[0].Id));

        workspaces.Create("Copied", WorkspaceSeed.Copy, "Scratch", copyJournal: true);
        Assert.Equal(2, workspaces.GetJournal().Entries.Count);
        Assert.Empty(workspaces.GetJournal().Versions);

        workspaces.Create("Fresh", WorkspaceSeed.Vocabularies, "Scratch");
        Assert.Empty(workspaces.GetJournal().Entries);
    }
}
