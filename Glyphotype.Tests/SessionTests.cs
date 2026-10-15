using Glyphotype.Distiller.Agent;
using Glyphotype.Distiller.Inspection;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary>Agent sessions: one brief to start from, step rules the tools enforce, and check-ins when they're due.</summary>
[Collection(CorpusCollection.Name)]
public sealed class SessionTests(CorpusFixture corpus) : IDisposable
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

    readonly string _directory = Directory.CreateTempSubdirectory("session-tests-").FullName;

    public void Dispose() => Directory.Delete(_directory, recursive: true);

    /// <summary>An agent over a corpus where snoring and yawning recur - so glyphs for them pay for themselves.</summary>
    GrammarAgent CreateAgent(AgentSessionSettings settings)
    {
        string[] animals = ["dog", "cat", "bird"];
        string[] places = ["kitchen", "garden", "barn"];
        var documents = Enumerable.Range(0, 30)
            .Select(i => new TestDocument(TestDocument.Unnamed, $"the {animals[i % 3]} snores in the {places[i / 3 % 3]}.\nthe {animals[i % 3]} yawns in the {places[i / 3 % 3]}.\nthe dog sleeps in the kitchen.", []));

        var workbench = _workbench = new GrammarWorkbench(corpus.Grammar, CorpusFixture.Process(corpus.Grammar, documents),
            new(Path.Combine(_directory, "working.json"), _directory, "Glyphotype.Tests.Grammar"));

        return new GrammarAgent(workbench, "a snoring corpus", settings);
    }

    /// <summary>The workbench the last agent made works on.</summary>
    GrammarWorkbench _workbench;

    [Fact]
    public async Task A_session_starts_from_one_brief_with_the_instructions_and_the_rules()
    {
        var agent = CreateAgent(new() { StepsBeforeCheckIn = 3 });

        var brief = await agent.StartSessionAsync("focus on snoring");

        Assert.Contains("The person's instructions: focus on snoring", brief);
        Assert.Contains("Check in after 3 applied steps", brief);
        Assert.Contains("The goal is full coverage", brief);
        Assert.Contains("A step must cover at least one more word, and take no words from a glyph that matches them", brief);
        Assert.Contains("Corpus: a snoring corpus", brief);
    }

    [Fact]
    public async Task A_step_that_covers_no_more_words_needs_the_persons_approval()
    {
        // Removing AnimalRests uncovers "the dog sleeps in the kitchen" on every document.
        var agent = CreateAgent(new());

        var refused = await agent.ApplyAsync(null, remove: "AnimalRests");
        Assert.StartsWith("Not applied - it takes words from AnimalRests (180 words), and a step must take none; it covers 180 fewer words", refused);
        Assert.Contains("check in", refused);

        Assert.Contains("`apply` would refuse this", await agent.EvaluateAsync(null, remove: "AnimalRests"));

        var approved = await agent.ApplyAsync(null, remove: "AnimalRests", description: "no resting", approval: "go ahead");
        Assert.Contains("Applied as step 1: no resting (approved: go ahead)", approved);
    }

    [Fact]
    public async Task A_step_that_takes_words_from_another_glyph_lists_them_and_is_refused()
    {
        // Tried first, it claims the lines AnimalRests matched: as many words covered, but taken from another glyph.
        const string dogSleeps = """
            [TokenizationOrder(0)]
            public class DogSleepsInTheKitchen : Glyph
            {
                public override Nib[] Nibs => ["the dog sleeps in the kitchen"];
            }
            """;
        var agent = CreateAgent(new());

        var evaluated = await agent.EvaluateAsync(dogSleeps);
        Assert.Contains("Taken from existing glyphs", evaluated);
        Assert.Contains("AnimalRests: 180 words on 30 lines, to DogSleepsInTheKitchen (180)", evaluated);
        Assert.Contains("→ ⟦DogSleepsInTheKitchen: the dog sleeps in the kitchen⟧", evaluated);

        Assert.StartsWith("Not applied - it takes words from AnimalRests (180 words)", await agent.ApplyAsync(dogSleeps));
    }

    [Fact]
    public void A_glyph_that_takes_over_a_dynamic_slot_takes_its_words_from_the_glyph_that_filled_it()
    {
        var definition = corpus.Grammar.ToDefinition();
        var glyph = Assert.Single(GlyphSourceReader.Read("""
            [TokenizationOrder(0)]
            public class DogSleepsInTheKitchen : Glyph
            {
                public override Nib[] Nibs => ["the dog sleeps in the kitchen"];
            }
            """, definition).Glyphs);
        var after = definition.WithGlyph(glyph);
        TestDocument[] documents = [new(TestDocument.Unnamed, "if it rains, the dog sleeps in the kitchen.", [])];

        var steals = CorpusQueries.FindSteals(
            CorpusFixture.Process(corpus.Grammar, documents),
            CorpusFixture.Process(GlyphGrammar.FromDefinition(after), documents),
            after.Glyphs.Select(x => x.Name).ToHashSet());

        // IfWeather still frames the line: only its slot changed hands.
        var steal = Assert.Single(steals);
        Assert.Equal("AnimalRests", steal.From);
        Assert.Equal(6, steal.Words);
        Assert.Equal([("DogSleepsInTheKitchen", 6)], steal.TakenBy);
    }

    [Fact]
    public async Task A_step_that_covers_more_words_and_takes_none_goes_through()
    {
        var agent = CreateAgent(new());

        var evaluated = await agent.EvaluateAsync(_animalSnores);
        Assert.DoesNotContain("Taken from existing glyphs", evaluated);
        Assert.DoesNotContain("would refuse", evaluated);

        Assert.StartsWith("Applied as step 1", await agent.ApplyAsync(_animalSnores));
    }

    [Fact]
    public async Task An_applied_step_records_what_it_did_to_the_score()
    {
        var agent = CreateAgent(new());
        var workbench = _workbench;
        var before = (await workbench.GetCurrentTrialAsync()).Score;

        await agent.ApplyAsync(_animalSnores);
        var after = (await workbench.GetCurrentTrialAsync()).Score;

        var change = Assert.Single(workbench.History).ScoreChange;
        Assert.Equal(after.TotalBits - before.TotalBits, change.Bits, 6);
        Assert.Equal(after.Coverage - before.Coverage, change.Coverage, 9);
        Assert.Equal(after.CapturedWords - before.CapturedWords, change.Words);
        Assert.Equal(after.CompressionRatio - before.CompressionRatio, change.Compression.Value, 9);
        Assert.True(change.Bits < 0 && change.Coverage > 0);
    }

    [Fact]
    public async Task After_the_configured_steps_the_agent_checks_in_until_the_session_resumes()
    {
        var agent = CreateAgent(new() { StepsBeforeCheckIn = 2 });
        await agent.StartSessionAsync();

        Assert.Contains("Session: 1 of 2 steps applied - keep going", await agent.ApplyAsync(_animalSnores));
        Assert.Contains("Check-in due (2 steps applied)", await agent.ApplyAsync(_animalYawns));

        var refused = await Assert.ThrowsAsync<AgentRequestException>(() => agent.ApplyAsync(_animalSnores.Replace("snores", "dozes").Replace("Snores", "Dozes")));
        Assert.Contains("check-in is due", refused.Message);

        await agent.StartSessionAsync("keep going");
        Assert.Contains("Session: 1 of 2 steps applied", await agent.UndoAndReapply(_animalYawns));
    }

    [Fact]
    public async Task An_agent_asked_to_stop_after_its_step_checks_in_once_it_applies_one()
    {
        var agent = CreateAgent(new() { StepsBeforeCheckIn = 0 });
        await agent.StartSessionAsync();
        agent.StopAfterStep();

        Assert.False(agent.IsCheckInDue);
        Assert.Contains("Check-in due (1 step applied, and the person asked you to stop after this one)", await agent.ApplyAsync(_animalSnores));
        Assert.True(agent.IsCheckInDue);

        var refused = await Assert.ThrowsAsync<AgentRequestException>(() => agent.ApplyAsync(_animalYawns));
        Assert.Contains("as the person asked", refused.Message);

        // A new session is a new round, without the stop.
        await agent.StartSessionAsync();
        Assert.False(agent.IsStopAfterStepRequested);
        Assert.DoesNotContain("Check-in due", await agent.ApplyAsync(_animalYawns));
    }

    [Fact]
    public async Task A_session_brief_states_the_focus_the_person_set()
    {
        var agent = CreateAgent(new());
        Assert.DoesNotContain("this session's focus", await agent.StartSessionAsync());

        agent.Settings = agent.Settings with { Focus = AgentFocus.HotSpots };
        Assert.Contains("The person has set this session's focus. Hot spots: work down `residuals`", await agent.StartSessionAsync());
    }

    [Fact]
    public async Task An_agent_that_keeps_missing_is_told_to_check_in()
    {
        var agent = CreateAgent(new() { AttemptsBeforeCheckIn = 2 });
        await agent.StartSessionAsync();

        Assert.DoesNotContain("Check-in due", await agent.EvaluateAsync(_animalSnores));
        Assert.Contains("Check-in due: 2 evaluations without an applied step", await agent.EvaluateAsync(_animalYawns));
    }
}

static class SessionTestExtensions
{
    /// <summary>Undoes the latest step and applies <paramref name="source"/> again - a fresh step in a resumed session.</summary>
    public static async Task<string> UndoAndReapply(this GrammarAgent agent, string source)
    {
        await agent.UndoAsync();
        return await agent.ApplyAsync(source);
    }
}
