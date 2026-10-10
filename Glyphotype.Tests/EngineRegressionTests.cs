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

    [Theory]
    [InlineData("a dog with a loud bark", "WithLoud")]
    [InlineData("a dog with size 2 or less", "WithSize")]
    [InlineData("a dog", null)]
    public void An_optional_one_of_matches_any_of_its_alternatives(string text, string expected)
    {
        // An optional one-of carries its leading joiner inside its own group, where it used to sit before the first
        // alternative alone: (?<With> (?<A>…)|(?<B>…))? - so the second never matched after a space.
        var grammar = Build("""
            [Dependent]
            public class WithLoud : Glyph
            {
                public override Nib[] Nibs => ["with a loud bark"];
            }

            [Dependent]
            public class WithSize : Glyph
            {
                public override Nib[] Nibs => ["with size", Pattern("[0-9]"), "or less"];
            }

            public class Dog : Glyph
            {
                public override Nib[] Nibs => ["a dog", Prop(With)];

                [Optional]
                public OneOf<WithLoud, WithSize> With { get; set; }
            }
            """);

        var match = Assert.IsAssignableFrom<Glyph>(Assert.Single(grammar.Tokenize(text)));
        var rendered = ParseRenderer.Render(match, nested: true);

        if (expected is null)
            Assert.DoesNotContain("With", rendered);
        else
            Assert.Contains($"⟦{expected}: ", rendered);
    }
}
