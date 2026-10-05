using Glyphotype.Definitions;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary>What each <see cref="BackReference"/> on a line is resolved to (see <see cref="BackReferenceResolver"/>), over the test grammar's back-reference glyphs.</summary>
[Collection(CorpusCollection.Name)]
public class BackReferenceTests(CorpusFixture corpus)
{
    ProcessedLine Process(string text) =>
        Assert.Single(ProcessedLine.GetAll(new TestDocument(TestDocument.Unnamed, text, []), corpus.Grammar));

    /// <summary>Each back-reference on the line as "back-reference → antecedent" (or "→ ∅" unresolved), in text order.</summary>
    List<string> Resolve(string text) =>
        Process(text).BackReferences.Select(x => $"{x.Trace.CaptureValue} → {x.Antecedent?.Text ?? "∅"}").ToList();

    [Fact]
    public void A_back_reference_takes_the_most_recent_agreeing_referent_across_clauses_and_glyphs()
    {
        Assert.Equal(["it → baker", "that animal → dog"], Resolve("the dog befriends the baker. it sleeps all day. that animal sleeps all day."));
    }

    [Fact]
    public void A_back_reference_skips_referents_of_the_wrong_number()
    {
        Assert.Equal(["they → apples"], Resolve("the baker buys 3 apples. they sleep all day."));
        Assert.Equal(["they → ∅"], Resolve("the dog befriends the baker. they sleep all day."));
    }

    [Fact]
    public void A_phrase_is_never_its_own_antecedent()
    {
        // "it" sits inside "the friends of it" - resolved before that phrase is introduced, so to the dog; "they"
        // then finds the phrase.
        Assert.Equal(["it → dog", "they → the friends of it"], Resolve("the dog meets the friends of it. they sleep all day."));
    }

    [Fact]
    public void RefersTo_binds_outright_over_a_more_recent_referent()
    {
        var resolution = Assert.Single(Process("the dog follows the baker until it rests.").BackReferences);

        Assert.Equal("dog", resolution.Antecedent.Text);
        Assert.Equal(Animal.Dog, resolution.Antecedent.Value);
        Assert.Equal(BackReferenceResolutionKind.Declared, resolution.Kind);
    }

    [Theory]
    [InlineData("the baker visits rex. it sleeps all day.")]
    [InlineData("the baker feeds rex. it sleeps all day.")]
    public void The_documents_reference_to_itself_is_a_referent_whether_literal_or_captured(string text)
    {
        var resolution = Assert.Single(Assert.Single(ProcessedLine.GetAll(new TestDocument("rex", text, []), corpus.Grammar)).BackReferences);

        Assert.Equal("{this}", resolution.Antecedent.Text);
        Assert.True(resolution.Antecedent.IsSelf);
        Assert.IsType<This>(resolution.Antecedent.Value);
    }

    [Fact]
    public void The_documents_reference_to_itself_is_singular()
    {
        var resolution = Assert.Single(Assert.Single(ProcessedLine.GetAll(new TestDocument("rex", "the baker visits rex. they sleep all day.", []), corpus.Grammar)).BackReferences);

        Assert.False(resolution.IsResolved);
    }

    [Fact]
    public void A_back_reference_with_nothing_to_refer_to_is_left_unresolved_and_counted()
    {
        var document = new ProcessedDocument(new TestDocument(TestDocument.Unnamed, "it sleeps all day.\nthe dog befriends the baker. it sleeps all day.", []), corpus.Grammar);

        Assert.Equal(2, document.BackReferenceCount);
        Assert.Equal(1, document.UnresolvedBackReferenceCount);
    }

    [Fact]
    public void Referents_do_not_outlive_their_line()
    {
        var lines = ProcessedLine.GetAll(new TestDocument(TestDocument.Unnamed, "the dog befriends the baker.\nit sleeps all day.", []), corpus.Grammar);

        Assert.False(Assert.Single(lines[1].BackReferences).IsResolved);
    }

    [Fact]
    public void Back_references_survive_a_round_trip_through_source()
    {
        // Agreement, Introduces and RefersTo are all declarative, so they're grammar a definition carries: written out
        // as C# source, read back and compiled, the grammar resolves exactly as the hand-written one does.
        var source = GlyphSourceWriter.Write(corpus.Grammar.ToDefinition(), "RoundTrip");
        var read = GlyphSourceReader.Read(source);
        var compiled = GlyphGrammar.FromDefinition(new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers }, allowPartialClauseMatches: false);

        Assert.Contains("[Agreement(GrammaticalNumber.Singular, \"animal\")]", source);
        Assert.Contains("[RefersTo(nameof(Animal))]", source);

        const string text = "the dog follows the baker until it rests. the dog meets the friends of it. they sleep all day. that animal sleeps all day.";
        string Resolutions(GlyphGrammar grammar) =>
            string.Join(", ", Assert.Single(ProcessedLine.GetAll(new TestDocument(TestDocument.Unnamed, text, []), grammar)).BackReferences.Select(x => $"{x.Trace.CaptureValue} → {x.Antecedent?.Text}"));

        Assert.Equal(Resolutions(corpus.Grammar), Resolutions(compiled));
    }

    [Theory]
    [InlineData("""
        [Agreement(GrammaticalNumber.Plural)]
        public class NotABackReference : Glyph { public override Nib[] Nibs => ["they"]; }
        """, "only a back-reference has a referent to agree with")]
    [InlineData("""
        [Dependent] public class Pronoun : BackReference { public override Nib[] Nibs => ["it"]; }
        public class Chases : Glyph
        {
            public override Nib[] Nibs => [Prop(Chaser), "chases", Prop(Chased)];
            public Pronoun Chaser { get; set; }
            [RefersTo(nameof(Chaser))]
            public Pronoun Chased { get; set; }
        }
        """, null)]
    [InlineData("""
        [Dependent] public class Pronoun : BackReference { public override Nib[] Nibs => ["it"]; }
        public class Chases : Glyph
        {
            public override Nib[] Nibs => ["the dog chases", Prop(Chased)];
            [RefersTo("Dog")]
            public Pronoun Chased { get; set; }
        }
        """, "names no other property")]
    [InlineData("""
        public class Pronoun : BackReference { public override Nib[] Nibs => ["it"]; }
        public class Chases : Glyph { public override Nib[] Nibs => ["the dog chases", Prop(Chased)]; public Pronoun Chased { get; set; } }
        """, "isn't [Dependent]")]
    [InlineData("""
        public class Chases : Glyph { public override Nib[] Nibs => ["the dog chases", Prop(Chased)]; public Them Chased { get; set; } }
        """, null)]
    public void Agreement_belongs_to_dependent_back_references_and_RefersTo_to_a_sibling(string source, string expectedError)
    {
        var read = GlyphSourceReader.Read(source);
        GlyphGrammar Build() => GlyphGrammar.FromDefinition(new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers }, allowPartialClauseMatches: false);

        if (expectedError is null)
            Build();
        else
            Assert.Contains(expectedError, Assert.ThrowsAny<Exception>(Build).Message);
    }
}
