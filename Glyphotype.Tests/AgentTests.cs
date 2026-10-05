using Glyphotype.Distiller.Agent;
using Glyphotype.Distiller.Inspection;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary><see cref="GrammarAgent"/>: the agent's loop over a workbench - orient, inspect, probe, evaluate, apply, undo.</summary>
[Collection(CorpusCollection.Name)]
public sealed class AgentTests(CorpusFixture corpus) : IDisposable
{
    const string _animalSnores = """
        public class AnimalSnores : Glyph
        {
            public override Nib[] Nibs => ["the", Prop(Animal), "snores in the", Prop(Place)];

            public Animal Animal { get; set; }
            public Place Place { get; set; }
        }
        """;

    readonly string _directory = Directory.CreateTempSubdirectory("agent-tests-").FullName;

    /// <summary>Session settings that let any step through and never call for a check-in.</summary>
    internal static readonly AgentSessionSettings AnySteps = new() { StepsBeforeCheckIn = 0, AttemptsBeforeCheckIn = 0, MinimumGainBits = double.NegativeInfinity, AllowLostLines = true };

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    (GrammarAgent Agent, GrammarWorkbench Workbench) CreateAgent()
    {
        var workbench = new GrammarWorkbench(corpus.Grammar, corpus.ProcessedDocuments,
            new(Path.Combine(_directory, "working.json"), _directory, "Glyphotype.Tests.Grammar", AllowPartialClauseMatches: false));

        // These tests exercise the tools themselves, so the session's step rules are off.
        return (new GrammarAgent(workbench, "the test corpus", AnySteps), workbench);
    }

    [Fact]
    public async Task The_overview_orients_on_the_score_and_the_costliest_unmatched_text()
    {
        var (agent, _) = CreateAgent();

        var overview = await agent.OverviewAsync();

        Assert.Contains("the test corpus", overview);
        Assert.Contains("of the no-grammar baseline", overview);
        Assert.Contains("Costliest unmatched text", overview);
        Assert.Contains("the doctor comes on monday at 930 am", overview);
        Assert.Contains("guide", overview);
    }

    [Fact]
    public void The_guide_is_embedded()
    {
        Assert.Contains("## The loop", GrammarAgent.Guide);
    }

    [Fact]
    public async Task Evaluating_leaves_the_working_definition_alone()
    {
        var (agent, workbench) = CreateAgent();

        var report = await agent.EvaluateAsync(_animalSnores);

        // One line's worth of matches doesn't pay for a glyph's own definition - the report says so rather than flattering it.
        Assert.Contains("Evaluated, not applied: set AnimalSnores", report);
        Assert.Contains("- worse", report);
        Assert.Contains("1 gained, 0 lost", report);
        Assert.Contains("«the dog snores in the kitchen»", report);
        Assert.Contains("→ ⟦AnimalSnores: the dog snores in the kitchen⟧", report);
        Assert.Contains("(added)", report);
        Assert.Empty(workbench.Changes);
        Assert.Empty(workbench.History);
    }

    [Fact]
    public async Task Applying_makes_a_step_that_undo_takes_back()
    {
        var (agent, workbench) = CreateAgent();

        var applied = await agent.ApplyAsync(_animalSnores, description: "snoring animals");

        Assert.Contains("Applied as step 1: snoring animals", applied);
        Assert.Contains("1 gained", applied);
        Assert.Contains(workbench.Changes, x => x.Name == "AnimalSnores");
        Assert.Contains("#1", agent.History());
        Assert.Contains("AnimalSnores (added)", agent.History());

        var matches = await agent.MatchesAsync("AnimalSnores");
        Assert.Contains("1 top-level", matches);
        Assert.Contains("the {Animal} snores in the {Place}", matches);

        var undone = await agent.UndoAsync();

        Assert.Contains("Undid step 1: snoring animals", undone);
        Assert.Contains("1 lost", undone);
        Assert.Empty(workbench.Changes);
    }

    [Fact]
    public async Task With_type_filters_off_a_change_introducing_a_marker_or_filter_is_refused_and_the_brief_says_why()
    {
        var (agent, workbench) = CreateAgent();
        agent.Settings = agent.Settings with { AllowTypeFilters = false };

        const string filtered = """
            public interface IRest { }

            public class AnimalRests : Glyph, IRest
            {
                public override Nib[] Nibs => ["the", Prop(Animal), "rests:", Prop(Then)];

                public Animal Animal { get; set; }

                [TypeFilter(typeof(IRest))]
                public DynamicGlyph Then { get; set; }
            }
            """;

        var refused = await Assert.ThrowsAsync<AgentRequestException>(() => agent.EvaluateAsync(filtered));
        Assert.Contains("declares the marker interface IRest", refused.Message);
        Assert.Contains("marks AnimalRests with IRest", refused.Message);
        Assert.Contains("filters AnimalRests.Then", refused.Message);
        Assert.Contains(GrammarAgent.TypeFilterPolicy, refused.Message);
        await Assert.ThrowsAsync<AgentRequestException>(() => agent.ApplyAsync(filtered));

        // Unfiltered, the same glyph is fine.
        await agent.ApplyAsync(filtered.Replace("public interface IRest { }", "").Replace(", IRest", "").Replace("[TypeFilter(typeof(IRest))]", ""));
        Assert.Contains(workbench.WorkingDefinition.Glyphs, x => x.Name == "AnimalRests");

        Assert.Contains(GrammarAgent.TypeFilterPolicy, await agent.StartSessionAsync());
    }

    [Fact]
    public async Task Suspended_check_ins_let_steps_through_as_a_round_of_their_own_and_leave_the_sessions_count_as_it_was()
    {
        static string Glyph(string name, string verb) => $$"""
            public class {{name}} : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal), "{{verb}} in the", Prop(Place)];

                public Animal Animal { get; set; }
                public Place Place { get; set; }
            }
            """;

        var (agent, workbench) = CreateAgent();
        agent.Settings = agent.Settings with { StepsBeforeCheckIn = 1 };

        await agent.StartSessionAsync();
        Assert.Contains("Check-in due", await agent.ApplyAsync(_animalSnores));

        using (agent.SuspendCheckIns())
        {
            var applied = await agent.ApplyAsync(Glyph("AnimalSleeps", "sleeps"));
            Assert.DoesNotContain("Check-in", applied);
            await agent.ApplyAsync(Glyph("AnimalNaps", "naps"));
        }

        Assert.Equal(1, workbench.ChangeRounds[(DefinitionKind.Glyph, "AnimalSnores")]);
        Assert.Equal(2, workbench.ChangeRounds[(DefinitionKind.Glyph, "AnimalSleeps")]);
        Assert.Equal(2, workbench.ChangeRounds[(DefinitionKind.Glyph, "AnimalNaps")]);

        // The session's own check-in is still due.
        await Assert.ThrowsAsync<AgentRequestException>(() => agent.ApplyAsync(Glyph("AnimalDozes", "dozes")));
    }

    [Fact]
    public async Task A_change_is_marked_with_the_round_an_agent_began_it_in()
    {
        static string Glyph(string name, string verb) => $$"""
            public class {{name}} : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal), "{{verb}} in the", Prop(Place)];

                public Animal Animal { get; set; }
                public Place Place { get; set; }
            }
            """;

        var (agent, workbench) = CreateAgent();

        // A person's own edit belongs to no round.
        workbench.Apply(ChangeSet.FromSource(Glyph("AnimalNaps", "naps"), [], workbench.WorkingDefinition));

        await agent.StartSessionAsync();
        await agent.ApplyAsync(_animalSnores);

        // Each session is a round of its own.
        await agent.StartSessionAsync();
        await agent.ApplyAsync(Glyph("AnimalSleeps", "sleeps"));
        await agent.ApplyAsync(Glyph("AnimalNaps", "dozes"));

        Assert.Equal(1, workbench.ChangeRounds[(DefinitionKind.Glyph, "AnimalSnores")]);
        Assert.Equal(2, workbench.ChangeRounds[(DefinitionKind.Glyph, "AnimalSleeps")]);
        Assert.DoesNotContain((DefinitionKind.Glyph, "AnimalNaps"), workbench.ChangeRounds.Keys);

        // Taking a round's steps back takes its marks with them.
        workbench.Undo();
        workbench.Undo();
        Assert.Equal([(DefinitionKind.Glyph, "AnimalSnores")], workbench.ChangeRounds.Keys);
    }

    [Fact]
    public async Task Reverting_a_round_reverts_only_the_changes_it_began()
    {
        static string Glyph(string name, string verb) => $$"""
            public class {{name}} : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal), "{{verb}} in the", Prop(Place)];

                public Animal Animal { get; set; }
                public Place Place { get; set; }
            }
            """;

        var (agent, workbench) = CreateAgent();

        // A person's own change, in no round.
        workbench.Apply(ChangeSet.FromSource(Glyph("AnimalDozes", "dozes"), [], workbench.WorkingDefinition));

        await agent.StartSessionAsync();
        await agent.ApplyAsync(_animalSnores);

        await agent.StartSessionAsync();
        await agent.ApplyAsync(Glyph("AnimalSleeps", "sleeps"));
        await agent.ApplyAsync(Glyph("AnimalNaps", "naps"));

        var beforeRevert = workbench.WorkingDefinition;
        workbench.RevertRound(2);

        Assert.Equal(["AnimalDozes", "AnimalSnores"], workbench.Changes.Select(x => x.Name).Order());
        Assert.Throws<InvalidOperationException>(() => workbench.RevertRound(2));

        // Restoring the earlier working grammar, as a page's undo does, brings the changes back under their round.
        workbench.Restore(beforeRevert, "undo revert AI round 2");
        Assert.Equal(4, workbench.Changes.Count);
        Assert.Equal(1, workbench.ChangeRounds[(DefinitionKind.Glyph, "AnimalSnores")]);
        Assert.Equal(2, workbench.ChangeRounds[(DefinitionKind.Glyph, "AnimalSleeps")]);
        Assert.Equal(2, workbench.ChangeRounds[(DefinitionKind.Glyph, "AnimalNaps")]);

        // The changes no round began revert together, leaving the rounds' own.
        workbench.RevertOtherChanges();
        Assert.Equal(["AnimalNaps", "AnimalSleeps", "AnimalSnores"], workbench.Changes.Select(x => x.Name).Order());
        Assert.Throws<InvalidOperationException>(workbench.RevertOtherChanges);
    }

    [Fact]
    public async Task A_change_that_wouldnt_build_is_refused()
    {
        var (agent, workbench) = CreateAgent();

        var report = await agent.ApplyAsync("""
            public class Clash : Glyph
            {
                public override Nib[] Nibs => ["the", Prop(Animal)];

                [Optional]
                public Animal Animal { get; set; }
            }
            """);

        Assert.StartsWith("Not applied", report);
        Assert.Contains("Clash", report);
        Assert.Empty(workbench.History);
    }

    [Fact]
    public async Task Bad_requests_say_what_to_do_instead()
    {
        var (agent, _) = CreateAgent();

        var unreadable = await Assert.ThrowsAsync<AgentRequestException>(() => agent.EvaluateAsync("public class X : Glyph { public void Y() { } }"));
        Assert.Contains("line 1", unreadable.Message);

        var undefined = await Assert.ThrowsAsync<AgentRequestException>(() => agent.EvaluateAsync("public class X : Glyph { public Missing M { get; set; } }"));
        Assert.Contains("X refers to Missing", undefined.Message);

        var referenced = await Assert.ThrowsAsync<AgentRequestException>(() => agent.EvaluateAsync(null, remove: "OnDay"));
        Assert.Contains("OnDay", referenced.Message);

        await Assert.ThrowsAsync<AgentRequestException>(() => agent.EvaluateAsync(" "));
        await Assert.ThrowsAsync<AgentRequestException>(() => agent.UndoAsync());
    }

    [Fact]
    public async Task Lines_can_be_searched_within_unmatched_text()
    {
        var (agent, _) = CreateAgent();

        var unmatched = await agent.SearchLinesAsync("snores", LineScope.Unmatched);
        Assert.Contains("«the dog snores in the kitchen»", unmatched);

        var byGlyph = await agent.SearchLinesAsync(null, glyph: "OnDay", nested: true);
        Assert.Contains("⟦Day: on ⟦Weekday=monday⟧⟧", byGlyph);
    }

    [Fact]
    public async Task Recurring_unmatched_phrases_are_found_at_their_longest()
    {
        var (agent, _) = CreateAgent();

        var phrases = await agent.ResidualPhrasesAsync(minWords: 3, minOccurrences: 2);

        Assert.Contains("the dog sleeps in the", phrases);
        Assert.DoesNotContain("  dog sleeps in the\r", phrases);
    }

    [Fact]
    public async Task Drafts_can_be_probed_before_they_are_evaluated()
    {
        var (agent, _) = CreateAgent();

        Assert.Contains("⟦AnimalSnores: the ⟦Animal=cat⟧ snores in the ⟦Place=barn⟧⟧",
            await agent.TokenizeAsync("The cat snores in the barn", _animalSnores));

        var explanation = await agent.ExplainMismatchAsync("AnimalSnores", "the cat snores in the attic", _animalSnores);
        Assert.Contains("matches 5 of 6 words", explanation);
        Assert.Contains("Place", explanation);
        Assert.Contains("«the cat snores in the attic»", explanation);
    }

    [Fact]
    public async Task Definitions_are_shown_as_source_and_listed_with_their_contributions()
    {
        var (agent, _) = CreateAgent();

        Assert.Contains("public class AnimalRests : Glyph", agent.Show("AnimalRests, Nope"));
        Assert.Contains("Nope: no glyph", agent.Show("AnimalRests, Nope"));

        var list = await agent.ListGlyphsAsync();
        Assert.Contains("AnimalRests", list);
        Assert.Contains("OnDay  (dependent)", list);
        Assert.Contains("Vocabularies", list);
    }

    [Fact]
    public async Task Every_glyph_can_be_removed_at_once_leaving_the_vocabularies_to_build_on()
    {
        var (agent, workbench) = CreateAgent();

        var applied = await agent.ApplyAsync(null, remove: string.Join(",", workbench.WorkingDefinition.Glyphs.Select(x => x.Name)), description: "start from scratch");

        Assert.Contains("Applied as step 1", applied);
        Assert.Empty(workbench.WorkingDefinition.Glyphs);
        Assert.NotEmpty(workbench.WorkingDefinition.Vocabularies);

        // Unused vocabularies cost nothing, so the score is the no-grammar baseline's...
        var trial = await workbench.GetCurrentTrialAsync();
        Assert.Equal(trial.Score.BaselineBits, trial.Score.TotalBits, precision: 6);

        // ...and a new glyph can use them straight away.
        Assert.Contains("Applied as step 2", await agent.ApplyAsync(_animalSnores));
    }
}
