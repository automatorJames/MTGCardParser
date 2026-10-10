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

    (GrammarAgent Agent, GrammarWorkbench Workbench) CreateAgent(AgentSessionSettings settings = null)
    {
        var workbench = new GrammarWorkbench(corpus.Grammar, corpus.ProcessedDocuments,
            new(Path.Combine(_directory, "working.json"), _directory, "Glyphotype.Tests.Grammar", AllowPartialClauseMatches: false));

        // These tests exercise the tools themselves, so the session's step rules are off unless a test sets them.
        return (new GrammarAgent(workbench, "the test corpus", settings ?? AnySteps), workbench);
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
    public async Task A_step_that_covers_more_words_may_cost_bits_up_to_a_price_per_word()
    {
        // Snoring costs bits here - one line's worth of matches doesn't pay for the glyph - but covers words nothing else does.
        var (dear, _) = CreateAgent(AnySteps with { MinimumGainBits = 1, MaxBitsPerCoveredWord = 0.001 });
        var refused = await dear.ApplyAsync(_animalSnores);
        Assert.StartsWith("Not applied - it takes", refused);
        Assert.Contains("per word it gains", refused);

        var (fair, _) = CreateAgent(AnySteps with { MinimumGainBits = 1, MaxBitsPerCoveredWord = 1_000 });
        Assert.StartsWith("Applied as step 1", await fair.ApplyAsync(_animalSnores));
    }

    [Fact]
    public async Task Evaluating_leaves_the_working_definition_alone()
    {
        var (agent, workbench) = CreateAgent();

        var report = await agent.EvaluateAsync(_animalSnores);

        // One line's worth of matches doesn't pay for a glyph's own definition - the report says what the coverage costs rather than flattering it.
        Assert.Contains("Evaluated, not applied: set AnimalSnores", report);
        Assert.Contains("- more bits, more coverage", report);
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
        Assert.Equal(3, workbench.ChangeSteps[(DefinitionKind.Glyph, "AnimalNaps")]);

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
    public async Task Applying_with_nothing_given_makes_the_change_last_evaluated_and_reports_it_in_brief()
    {
        var (agent, workbench) = CreateAgent();

        await Assert.ThrowsAsync<AgentRequestException>(() => agent.ApplyAsync(null));

        Assert.Contains("`apply` with no source or removals", await agent.EvaluateAsync(_animalSnores));

        var applied = await agent.ApplyAsync(null, description: "snoring animals");

        Assert.StartsWith("Applied as step 1: snoring animals", applied);
        Assert.Contains("As evaluated:", applied);
        Assert.DoesNotContain("Glyph contributions", applied);
        Assert.Contains(workbench.WorkingDefinition.Glyphs, x => x.Name == "AnimalSnores");

        // Applied once, the evaluation is spent: there's nothing left to make.
        await Assert.ThrowsAsync<AgentRequestException>(() => agent.ApplyAsync(null));
    }

    [Fact]
    public async Task An_evaluation_goes_stale_once_the_working_definition_changes()
    {
        var (agent, workbench) = CreateAgent();

        await agent.EvaluateAsync(_animalSnores);
        workbench.RemoveGlyph(workbench.WorkingDefinition.Glyphs.First(x => x.Name != "AnimalSnores").Name);

        var refused = await Assert.ThrowsAsync<AgentRequestException>(() => agent.ApplyAsync(null));
        Assert.Contains("changed since your last `evaluate`", refused.Message);

        // Given again in full, it's scored afresh and reported in full.
        Assert.Contains("Glyph contributions", await agent.ApplyAsync(_animalSnores));
    }

    [Fact]
    public async Task Uncovered_text_is_grouped_by_how_it_opens()
    {
        var (agent, _) = CreateAgent();

        var openings = await agent.UnmatchedOpeningsAsync(maxWords: 3, minOccurrences: 2);

        // Ranked by the uncovered words the spans hold: every "the dog sleeps" span is a "the dog" span too.
        Assert.Contains("the dog sleeps …  →  the dog sleeps in the garage", openings);
        Assert.True(openings.IndexOf("the dog …", StringComparison.Ordinal) < openings.IndexOf("the dog sleeps …", StringComparison.Ordinal));
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
    public async Task A_draft_that_cant_be_built_is_reported_by_every_probe_as_a_message()
    {
        var (agent, _) = CreateAgent();

        var optionalProperty = _animalSnores.Replace("Prop(Place)", "Opt(Prop(Place))");

        var tokenize = await Assert.ThrowsAsync<AgentRequestException>(() => agent.TokenizeAsync("the cat snores in the barn", optionalProperty));
        Assert.Contains("make a property optional with [Optional] on it instead", tokenize.Message);

        var explain = await Assert.ThrowsAsync<AgentRequestException>(() => agent.ExplainMismatchAsync("AnimalSnores", "the cat snores in the barn", optionalProperty));
        Assert.Contains("make a property optional with [Optional] on it instead", explain.Message);

        // Read, but refused as it's built: whatever the failure, the probe reports it rather than throwing it.
        var circular = await Assert.ThrowsAsync<AgentRequestException>(() => agent.TokenizeAsync("the cat snores in the barn", """
            public class Loop : Glyph
            {
                public override Nib[] Nibs => ["loop", Prop(Inner)];

                public Loop Inner { get; set; }
            }
            """));
        Assert.Contains("The draft doesn't build", circular.Message);
        Assert.Contains("Loop", circular.Message);
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

    [Fact]
    public async Task When_glyphs_are_to_be_documented_the_agent_is_reminded_of_one_that_isnt_and_documenting_goes_through()
    {
        var workbench = new GrammarWorkbench(corpus.Grammar, corpus.ProcessedDocuments,
            new(Path.Combine(_directory, "working.json"), _directory, "Glyphotype.Tests.Grammar", AllowPartialClauseMatches: false));
        var agent = new GrammarAgent(workbench, "the test corpus", AnySteps with { MinimumGainBits = 1, DocumentGlyphs = true });

        Assert.Contains("<exampledoc>", await agent.StartSessionAsync());

        // Undocumented: a reminder, not a refusal.
        Assert.Contains("Documentation: AnimalSnores lacks", await agent.EvaluateAsync(_animalSnores));
        var applied = await agent.ApplyAsync(_animalSnores, overrideReason: "requested");
        Assert.Contains("Applied as step 1", applied);
        Assert.Contains("Documentation: AnimalSnores lacks", applied);

        const string documentation = """
            /// <summary>An animal snoring somewhere.</summary>
            /// <exampledoc>Pet diary</exampledoc>
            /// <examplecapture>the dog snores in the garden</examplecapture>

            """;

        // Documenting it later changes nothing else, so takes no bits off - and needn't.
        applied = await agent.ApplyAsync(documentation + _animalSnores);
        Assert.Contains("Applied as step 2", applied);
        Assert.DoesNotContain("Documentation:", applied);

        Assert.Contains("Applied as step 3", await agent.ApplyAsync(documentation.Replace("somewhere", "in a place") + _animalSnores));
        Assert.Equal("An animal snoring in a place.", workbench.WorkingDefinition.Glyphs.Single(x => x.Name == "AnimalSnores").Documentation.Summary);
    }

    [Fact]
    public async Task A_step_that_only_documents_isnt_one_of_the_rounds_steps()
    {
        var (agent, _) = CreateAgent(AnySteps with { StepsBeforeCheckIn = 1 });

        Assert.Contains("Check-in due", await agent.ApplyAsync(_animalSnores));
        var refused = await Assert.ThrowsAsync<AgentRequestException>(() => agent.ApplyAsync(_animalSnores.Replace("Snores", "Dozes").Replace("snores", "dozes")));
        Assert.StartsWith("Not applied: a check-in is due", refused.Message);

        // Documenting what the round did is still allowed, and the check-in is still due after it.
        var documented = await agent.ApplyAsync("/// <summary>An animal snoring somewhere.</summary>" + Environment.NewLine + _animalSnores);
        Assert.Contains("Applied as step 2", documented);
        Assert.Contains("Check-in due (1 step applied)", documented);
    }

    [Fact]
    public async Task A_doc_comment_after_the_attributes_is_reported_as_dropped()
    {
        var (agent, _) = CreateAgent();

        var report = await agent.EvaluateAsync("[TokenizationOrder(0)]" + Environment.NewLine + "/// <summary>An animal snoring somewhere.</summary>" + Environment.NewLine + _animalSnores);
        Assert.Contains("Documentation dropped: the `///` doc comment on AnimalSnores comes after its attributes", report);

        Assert.DoesNotContain("Documentation dropped", await agent.EvaluateAsync("/// <summary>An animal snoring somewhere.</summary>" + Environment.NewLine + "[TokenizationOrder(0)]" + Environment.NewLine + _animalSnores));
    }
}
