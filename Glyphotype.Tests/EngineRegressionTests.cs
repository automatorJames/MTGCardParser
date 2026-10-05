using Glyphotype.Definitions;
using Glyphotype.Distiller.Inspection;
using Glyphotype.Distiller.Scoring;
using Glyphotype.Distiller.Workbench;
using Glyphotype.RegexGeneration.Graph.Nodes;

namespace Glyphotype.Tests;

/// <summary>Engine bugs found by composing grammars, each pinned by the smallest grammar that showed it.</summary>
public class EngineRegressionTests
{
    static GlyphGrammar Build(string source)
    {
        var read = GlyphSourceReader.Read(source);
        return GlyphGrammar.FromDefinition(new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers }, allowPartialClauseMatches: false);
    }

    const string _costs = """
        public enum CostSymbolKind { [RegexPattern("t")] Tap, [RegexPattern("c")] Colorless, [RegexPattern("g")] Green }

        public interface IEffect { }

        [Dependent]
        public class CostSymbol : Glyph
        {
            public override Joiner Joiner => Joiner.None;
            public override Nib[] Nibs => ["{", Prop(Symbol), "}"];
            public CostSymbolKind Symbol { get; set; }
        }

        [Dependent]
        [JoinedBy(Joiner.None)]
        public class SymbolRun : CompoundOf<CostSymbol>;

        [Dependent]
        public class AddMana : Glyph, IEffect
        {
            public override Nib[] Nibs => ["add", Prop(Mana)];
            public SymbolRun Mana { get; set; }
        }

        public class Activated : Glyph
        {
            public override Joiner Joiner => Joiner.None;
            public override Nib[] Nibs => [Prop(Cost), ": ", Prop(Effect)];
            public SymbolRun Cost { get; set; }

            [TypeFilter(typeof(IEffect))]
            public DynamicGlyph Effect { get; set; }
        }
        """;

    [Theory]
    [InlineData("{t}: add {c}{c}{g}")]
    [InlineData("{t}: add {c}{c}{c}")]
    public void Repeated_items_inside_a_dynamics_resolution_keep_their_values_and_names(string text)
    {
        // A repeated list's later items are read through narrowed views of their traces. Inside a dynamic's
        // resolution, those traces are renamed and moved into the enclosing match - and views made afterwards used to
        // start over from the graph node: the resolution's own names, and no values.
        var grammar = Build(_costs);
        var root = grammar.Tokenize(text).Single().CaptureContext.RootCaptureTrace;

        var symbols = ParseRenderer.SelfAndDescendants(root).Where(x => x.SourceNode is EnumNode).ToList();

        Assert.Equal(4, symbols.Count);
        Assert.All(symbols, x => Assert.NotNull(x.ClrValue));
        Assert.All(symbols.Skip(1), x => Assert.StartsWith("Activated_Effect_AddMana_", x.FullyQualifiedName));
        Assert.Equal(text.EndsWith("{g}") ? "Green" : "Colorless", symbols[^1].ClrValue.ToString());

        // What first surfaced it: the scorer reads every value.
        MdlScorer.Score(grammar, CorpusFixture.Process(grammar, [new TestDocument(TestDocument.Unnamed, text + ".", [])]));
    }

    [Theory]
    [InlineData("it runs quickly", "Quickly")]
    [InlineData("it runs slowly", "Slowly")]
    public void A_one_of_whose_alternatives_are_bools_matches_one_of_them(string text, string expected)
    {
        // A bool is an optional group - which, as a one-of alternative, took the separating pipe inside itself,
        // (?<Slowly>|slowly)?, matching nothing alongside every real alternative.
        var grammar = Build("""
            [Dependent]
            public class Speed : GlyphOneOf
            {
                [RegexPattern("quickly")]
                public bool Quickly { get; set; }

                [RegexPattern("slowly")]
                public bool Slowly { get; set; }
            }

            public class Runs : Glyph
            {
                public override Nib[] Nibs => ["it runs", Prop(Speed)];
                public Speed Speed { get; set; }
            }
            """);

        var match = Assert.IsAssignableFrom<Glyph>(Assert.Single(grammar.Tokenize(text)));
        var speed = match.Type.GetProperty("Speed").GetValue(match);

        foreach (var alternative in new[] { "Quickly", "Slowly" })
            Assert.Equal(alternative == expected, (bool)speed.GetType().GetProperty(alternative).GetValue(speed));

        Assert.IsType<UnmatchedString>(Assert.Single(grammar.Tokenize("it runs")));
    }

    const string _moods = """
        public enum Mood { Sleepy, Grumpy, Happy }

        public class DogNaps : Glyph
        {
            public override Nib[] Nibs => ["the dog", Prop(Mood), "naps"];

            [Optional]
            public Mood? Mood { get; set; }
        }

        public class MoodyDog : Glyph
        {
            public override Nib[] Nibs => [Prop(Mood), "dog barks"];

            [Optional]
            public Mood? Mood { get; set; }
        }
        """;

    [Theory]
    [InlineData("the dog sleepy naps", "Sleepy")]
    [InlineData("the dog grumpy naps", "Grumpy")]
    [InlineData("the dog happy naps", "Happy")]
    [InlineData("the dog naps", null)]
    [InlineData("sleepy dog barks", "Sleepy")]
    [InlineData("grumpy dog barks", "Grumpy")]
    [InlineData("happy dog barks", "Happy")]
    [InlineData("dog barks", null)]
    public void An_optional_vocabulary_matches_and_hydrates_every_member(string text, string expected)
    {
        // An optional property carries its separator inside its own group - and an enum's group is its members,
        // pipe-joined, so the separator bound to one member alone: (?<Mood>[ ]sleepy|grumpy|happy)?. The others then
        // didn't match, and the member carrying a trailing separator captured it ("happy "), which no member's
        // anchored pattern read back.
        var grammar = Build(_moods);

        var match = Assert.IsAssignableFrom<Glyph>(Assert.Single(grammar.Tokenize(text)));

        Assert.Equal(expected, match.Type.GetProperty("Mood").GetValue(match)?.ToString());
    }

    [Theory]
    [InlineData("wait 5 minutes", 5)]
    [InlineData("wait minutes", null)]
    public void An_optional_number_hydrates_without_its_separator(string text, int? expected)
    {
        // The separator an optional property carries is captured with it - " 5" - and an int parses only digits.
        var grammar = Build("""
            public class Wait : Glyph
            {
                public override Nib[] Nibs => ["wait", Prop(Minutes), "minutes"];

                [Optional]
                public int? Minutes { get; set; }
            }
            """);

        var match = Assert.IsAssignableFrom<Glyph>(Assert.Single(grammar.Tokenize(text)));

        Assert.Equal(expected, (int?)match.Type.GetProperty("Minutes").GetValue(match));
    }

    [Theory]
    [InlineData("pet the cat", "Cat")]
    [InlineData("pet the dog", "Dog")]
    [InlineData("pet the", null)]
    public void An_optional_one_of_matches_every_alternative(string text, string expected)
    {
        // A one-of's group is its alternatives, pipe-joined, the same as a vocabulary's.
        var grammar = Build("""
            public enum Cat { Cat }
            public enum Dog { Dog }

            [Dependent]
            public class Pet : GlyphOneOf
            {
                public Cat? Cat { get; set; }
                public Dog? Dog { get; set; }
            }

            public class PetThe : Glyph
            {
                public override Nib[] Nibs => ["pet the", Prop(Pet)];

                [Optional]
                public Pet Pet { get; set; }
            }
            """);

        var match = Assert.IsAssignableFrom<Glyph>(Assert.Single(grammar.Tokenize(text)));
        var pet = match.Type.GetProperty("Pet").GetValue(match);

        Assert.Equal(expected, pet is null ? null : new[] { "Cat", "Dog" }.Single(x => pet.GetType().GetProperty(x).GetValue(pet) is not null));
    }
    [Theory]
    [InlineData("the dog naps, where it's warm", "the dog naps by (the fire)")]
    [InlineData("the dog naps", "the dog naps by (the fire)")]
    public void A_nested_glyphs_edge_punctuation_binds_as_if_written_in_place(string opensWithComma, string closesWithParen)
    {
        // Punctuation decides the separator next to it - but only literal text was consulted, so a nested glyph
        // opening with "," was separated from what came before it like a word ("naps , where"), and one closing with
        // "(" from what came after it ("( the").
        var grammar = Build("""
            [Dependent]
            public class WhereItsWarm : Glyph
            {
                public override Nib[] Nibs => [",", "where it's warm"];
            }

            public class DogNaps : Glyph
            {
                public override Nib[] Nibs => ["the dog naps", Prop(Where)];

                [Optional]
                public WhereItsWarm Where { get; set; }
            }

            [Dependent]
            public class ByThe : Glyph
            {
                public override Nib[] Nibs => ["by", "("];
            }

            public class DogNapsBy : Glyph
            {
                public override Nib[] Nibs => ["the dog naps", Prop(By), "the fire)"];

                public ByThe By { get; set; }
            }
            """);

        var comma = Assert.IsAssignableFrom<Glyph>(Assert.Single(grammar.Tokenize(opensWithComma)));
        Assert.Equal("DogNaps", comma.Type.Name);
        Assert.Equal(opensWithComma.Contains("where"), comma.Type.GetProperty("Where").GetValue(comma) is not null);

        Assert.Equal("DogNapsBy", Assert.IsAssignableFrom<Glyph>(Assert.Single(grammar.Tokenize(closesWithParen))).Type.Name);
    }
}
