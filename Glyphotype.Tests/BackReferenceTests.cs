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
    public void The_documents_reference_to_itself_is_a_referent_whether_embedded_or_held_by_a_property(string text)
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
    public void A_back_reference_of_one_kind_skips_the_documents_reference_to_itself()
    {
        var resolution = Assert.Single(Assert.Single(ProcessedLine.GetAll(new TestDocument("rex", "the baker visits rex. that person waves.", []), corpus.Grammar)).BackReferences);

        Assert.Equal("baker", resolution.Antecedent.Text);
        Assert.Equal(typeof(Person), resolution.Antecedent.Kind);
    }

    [Fact]
    public void Definitions_saved_with_Introduces_and_Agreement_still_load()
    {
        var grammar = GrammarDefinition.FromJson("""
            {
              "Glyphs": [
                { "Name": "Phrase", "Kind": "Glyph", "Introduces": { "Number": "Plural", "Kind": "fruit" },
                  "Properties": [ { "Name": "Fruit", "Type": { "Kind": "Vocabulary", "Name": "Fruit" }, "Introduces": { "Number": "Singular" } } ] },
                { "Name": "ThoseFruit", "Kind": "BackReference", "Agreement": { "Number": "Plural", "Kind": "fruit" } }
              ]
            }
            """);

        Assert.True(grammar.Glyphs[0].IsReferent);
        Assert.Equal(GrammaticalNumber.Plural, grammar.Glyphs[0].Number);
        Assert.True(grammar.Glyphs[0].Properties[0].IsReferent);
        Assert.Equal(GrammaticalNumber.Singular, grammar.Glyphs[0].Properties[0].Number);
        Assert.Equal(GrammaticalNumber.Plural, grammar.Glyphs[1].Number);
        Assert.Null(grammar.Glyphs[1].ReferenceKind);
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
        // Referent, Singular/Plural, BackReference<T> and RefersTo are all declarative, so they're grammar a definition
        // carries: written out as C# source, read back and compiled, the grammar resolves exactly as the hand-written one does.
        var source = GlyphSourceWriter.Write(corpus.Grammar.ToDefinition(), "RoundTrip");
        var read = GlyphSourceReader.Read(source);
        var compiled = GlyphGrammar.FromDefinition(new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers }, allowPartialClauseMatches: false);

        Assert.Contains("public class ThatAnimal : BackReference<Animal>", source);
        Assert.Contains("[Referent(GrammaticalNumber.Plural)]", source);
        Assert.Contains("[RefersTo(nameof(Animal))]", source);

        const string text = "the dog follows the baker until it rests. the dog meets the friends of it. they sleep all day. that animal sleeps all day. the baker visits {this}. that person waves.";
        string Resolutions(GlyphGrammar grammar) =>
            string.Join(", ", Assert.Single(ProcessedLine.GetAll(new TestDocument(TestDocument.Unnamed, text, []), grammar)).BackReferences.Select(x => $"{x.Trace.CaptureValue} → {x.Antecedent?.Text}"));

        Assert.Equal(Resolutions(corpus.Grammar), Resolutions(compiled));
    }

    [Theory]
    [InlineData("""
        [Plural]
        public class NotABackReference : Glyph { public override Nib[] Nibs => ["they"]; }
        """, "[Plural], which does nothing here")]
    [InlineData("""
        [Dependent] [Singular, Plural] public class Pronoun : BackReference { public override Nib[] Nibs => ["it"]; }
        public class Naps : Glyph { public override Nib[] Nibs => [Prop(Napper), "naps"]; public Pronoun Napper { get; set; } }
        """, "is both [Singular] and [Plural]")]
    [InlineData("""
        [Referent] [Plural]
        public class Pack : Glyph { public override Nib[] Nibs => ["the pack"]; }
        """, "a referent's number goes in its own attribute, [Referent(GrammaticalNumber.Plural)]")]
    [InlineData("""
        public enum Pet { Dog, Cat }
        public class Adopts : Glyph { public override Nib[] Nibs => ["we adopt a", Prop(Pet)]; [Referent] [Singular] public Pet Pet { get; set; } }
        """, "a referent's number goes in its own attribute, [Referent(GrammaticalNumber.Singular)]")]
    [InlineData("""
        public class Barks : Glyph { public override Nib[] Nibs => ["the dog barks", Prop(Loudly)]; [Referent] [RegexPattern("loudly")] public bool Loudly { get; set; } }
        """, "a referent must capture an enum or a glyph")]
    [InlineData("""
        public enum Pet { Dog, Cat }
        [Dependent] public class ThatPet : BackReference<Pet> { public override Nib[] Nibs => ["that pet"]; }
        public class Naps : Glyph { public override Nib[] Nibs => [Prop(Napper), "naps"]; public ThatPet Napper { get; set; } }
        """, "nothing in the grammar is a [Referent] of kind Pet")]
    [InlineData("""
        public enum Pet { Dog, Cat }
        [Dependent] public class ThatPet : BackReference<Pet> { public override Nib[] Nibs => ["that pet"]; }
        public class Naps : Glyph { public override Nib[] Nibs => [Prop(Napper), "naps"]; public ThatPet Napper { get; set; } }
        public class Adopts : Glyph { public override Nib[] Nibs => ["we adopt a", Prop(Pet)]; [Referent] public Pet Pet { get; set; } }
        """, null)]
    [InlineData("""
        public enum Pet { Dog, Cat }
        [Dependent] public class ThatPet : BackReference<Pet> { public override Nib[] Nibs => ["that", Prop(Pet)]; [Referent] public Pet Pet { get; set; } }
        public class Naps : Glyph { public override Nib[] Nibs => [Prop(Napper), "naps"]; public ThatPet Napper { get; set; } }
        """, "would make ThatPet its own antecedent")]
    [InlineData("""
        [Dependent] [Referent] public class Pronoun : BackReference { public override Nib[] Nibs => ["it"]; }
        public class Naps : Glyph { public override Nib[] Nibs => [Prop(Napper), "naps"]; public Pronoun Napper { get; set; } }
        """, "already stands for what it refers to")]
    [InlineData("""
        [Dependent] public class ThatNumber : BackReference<int> { public override Nib[] Nibs => ["that number"]; }
        public class Naps : Glyph { public override Nib[] Nibs => [Prop(Napper), "naps"]; public ThatNumber Napper { get; set; } }
        """, "a back-reference's kind must be an enum or a glyph type")]
    [InlineData("""
        public enum Pet { Dog, Cat }
        public enum Owner { Alice, Bob }
        [Dependent] public class ThatPet : BackReference<Pet> { public override Nib[] Nibs => ["that pet"]; }
        public class Walks : Glyph
        {
            public override Nib[] Nibs => [Prop(Owner), "walks", Prop(Walked)];
            public Owner Owner { get; set; }
            [RefersTo(nameof(Owner))]
            public ThatPet Walked { get; set; }
        }
        """, "never a Pet")]
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
    public void Referents_and_back_references_are_validated(string source, string expectedError)
    {
        // A mistake is caught where it can first be seen: reading the source, or else building the grammar.
        void Build()
        {
            var read = GlyphSourceReader.Read(source);
            GlyphGrammar.FromDefinition(new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers }, allowPartialClauseMatches: false);
        }

        if (expectedError is null)
            Build();
        else
            Assert.Contains(expectedError, Assert.ThrowsAny<Exception>(Build).Message);
    }
}
