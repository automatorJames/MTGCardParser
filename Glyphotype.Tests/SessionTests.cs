using Glyphotype.Distiller.Agent;
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

        var workbench = new GrammarWorkbench(corpus.Grammar, CorpusFixture.Process(corpus.Grammar, documents),
            new(Path.Combine(_directory, "working.json"), _directory, "Glyphotype.Tests.Grammar", AllowPartialClauseMatches: false));

        return new GrammarAgent(workbench, "a snoring corpus", settings);
    }

    [Fact]
    public async Task A_session_starts_from_one_brief_with_the_instructions_and_the_rules()
    {
        var agent = CreateAgent(new() { StepsBeforeCheckIn = 3, MinimumGainBits = 5 });

        var brief = await agent.StartSessionAsync("focus on snoring");

        Assert.Contains("The person's instructions: focus on snoring", brief);
        Assert.Contains("Check in after 3 applied steps", brief);
        Assert.Contains("The goal is full coverage", brief);
        Assert.Contains("at least 5 bits off the total, or cover more words for at most 8 bits per word gained, and lose no lines", brief);
        Assert.Contains("Corpus: a snoring corpus", brief);
    }

    [Fact]
    public async Task A_step_that_breaks_the_rules_needs_a_reason()
    {
        var agent = CreateAgent(new() { MinimumGainBits = 1_000_000, MaxBitsPerCoveredWord = 0 });

        var refused = await agent.ApplyAsync(_animalSnores);
        Assert.StartsWith("Not applied - it takes", refused);

        var evaluated = await agent.EvaluateAsync(_animalSnores);
        Assert.Contains("`apply` would refuse this", evaluated);

        var overridden = await agent.ApplyAsync(_animalSnores, description: "snoring", overrideReason: "testing the override");
        Assert.Contains("Applied as step 1: snoring (override: testing the override)", overridden);
    }

    [Fact]
    public async Task A_step_that_loses_lines_is_refused_unless_lost_lines_are_allowed()
    {
        // Removing AnimalRests loses "the dog sleeps in the kitchen" on every document.
        var strict = CreateAgent(new() { MinimumGainBits = double.NegativeInfinity });
        Assert.Contains("loses 30 lines", await strict.ApplyAsync(null, remove: "AnimalRests"));

        var lenient = CreateAgent(new() { MinimumGainBits = double.NegativeInfinity, AllowLostLines = true });
        Assert.StartsWith("Applied", await lenient.ApplyAsync(null, remove: "AnimalRests"));
    }

    [Fact]
    public async Task After_the_configured_steps_the_agent_checks_in_until_the_session_resumes()
    {
        var agent = CreateAgent(new() { StepsBeforeCheckIn = 2 });
        await agent.StartSessionAsync();

        Assert.Contains("Session: step 1 of 2 before checking in", await agent.ApplyAsync(_animalSnores));
        Assert.Contains("Check-in due (2 steps applied)", await agent.ApplyAsync(_animalYawns));

        var refused = await Assert.ThrowsAsync<AgentRequestException>(() => agent.ApplyAsync(_animalSnores.Replace("snores", "dozes").Replace("Snores", "Dozes")));
        Assert.Contains("check-in is due", refused.Message);

        await agent.StartSessionAsync("keep going");
        Assert.Contains("Session: step 1 of 2", await agent.UndoAndReapply(_animalYawns));
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
