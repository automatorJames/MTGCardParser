namespace Glyphotype.Tests;

/// <summary><see cref="ProcessedLine.Clauses"/>: a line's tokens grouped into the clauses they came from.</summary>
[Collection(CorpusCollection.Name)]
public class LineClauseTests(CorpusFixture corpus)
{
    ProcessedLine Process(string text) =>
        Assert.Single(ProcessedLine.GetAll(new TestDocument(TestDocument.Unnamed, text, []), corpus.Grammar));

    static string Signature(LineClause clause) =>
        string.Join(" ", clause.Units.Select(GlyphSignature.Of));

    [Fact]
    public void Each_clause_holds_its_own_units_and_the_break_that_closes_it()
    {
        var line = Process("the dog sleeps in the kitchen. the cat eats fish. the dog snores");

        Assert.Equal(["AnimalRests{Animal=Dog, Place=Kitchen}", "AnimalEats{Animal=Cat, Food=Fish}", "«the dog snores»"], line.Clauses.Select(Signature));
        Assert.Equal([".", ".", null], line.Clauses.Select(x => x.Break?.CaptureValue));
    }

    [Fact]
    public void A_glyph_spanning_clauses_keeps_them_together()
    {
        var line = Process("the dog wakes up. then it eats bread. the bird sings loudly.");

        Assert.Equal(["MorningRoutine{Animal=Dog, Food=Bread}", "AnimalSings{Animal=Bird}"], line.Clauses.Select(Signature));
    }

    [Fact]
    public void A_closing_quote_belongs_to_the_break_before_it()
    {
        var line = Process("\"the cat eats fish.\" the bird sings loudly.");

        Assert.Equal([".\"", "."], line.Clauses.Select(x => x.Break?.CaptureValue));
    }
}
