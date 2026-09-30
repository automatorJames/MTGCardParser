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

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    (GrammarAgent Agent, GrammarWorkbench Workbench) CreateAgent()
    {
        var workbench = new GrammarWorkbench(corpus.Grammar, corpus.ProcessedDocuments,
            new(Path.Combine(_directory, "working.json"), _directory, "Glyphotype.Tests.Grammar", AllowPartialSegmentMatches: false));

        return (new GrammarAgent(workbench, "the test corpus"), workbench);
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
}
