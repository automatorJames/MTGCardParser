using Glyphotype.Definitions;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary>
/// The nib helpers are spaced consistently (see <see cref="JoinerRules"/>): the engine separates neighbours, an
/// optional nib's separator renders exactly when it does, punctuation binds to the token before it, and whatever
/// spacing an author writes into a nib is taken as written rather than doubled.
/// </summary>
public class NibSpacingTests
{
    static GlyphGrammar Build(string nibs)
    {
        var source = $$"""
            public enum Pet { Dog, Cat }
            public enum Toy { Ball, Rope }

            public class Spaced : Glyph
            {
                public override Nib[] Nibs => {{nibs}};
                {{(nibs.Contains("Prop(Pet)") ? "public Pet Pet { get; set; }" : "")}}
                {{(nibs.Contains("Prop(Toy)") ? "public Toy Toy { get; set; }" : "")}}
                {{(nibs.Contains("Prop(Count)") ? "public int Count { get; set; }" : "")}}
            }
            """;

        var read = GlyphSourceReader.Read(source);
        var definition = new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers };

        return GlyphGrammar.FromDefinition(definition, allowPartialClauseMatches: false, allowPeriodsInLiteralNibs: true);
    }

    static bool Matches(GlyphGrammar grammar, string line) =>
        grammar.Tokenize(line) is [{ } only] && only.GetType().Name == "Spaced";

    [Theory]
    [InlineData("""["the", Prop(Pet), "eats", Opt("the"), Prop(Toy)]""", "the dog eats the ball|the dog eats ball", "the dog eatsball|the dog eats  ball")]
    [InlineData("""["the", Prop(Pet), "eats", Opt("quickly")]""", "the dog eats|the dog eats quickly", "the dog eats ")]
    [InlineData("""[Opt("the"), Prop(Pet), "naps"]""", "the dog naps|dog naps", "thedog naps")]
    [InlineData("""[Opt("the"), Opt("big"), Prop(Pet), "naps"]""", "the big dog naps|the dog naps|big dog naps|dog naps", "thedog naps|the  dog naps")]
    [InlineData("""["the", Opt("very"), Opt("big"), Prop(Pet), "naps"]""", "the very big dog naps|the big dog naps|the very dog naps|the dog naps", "thedog naps")]
    [InlineData("""[Prop(Pet), Opt(","), Prop(Toy)]""", "dog, ball|dog ball", "dog , ball|dog,ball")]
    [InlineData("""[Prop(Pet), Opt("'s"), Prop(Toy)]""", "dog's ball|dog ball", "dog 's ball")]
    [InlineData("""[Prop(Pet), Alt(",", ";"), Prop(Toy)]""", "dog, ball|dog; ball", "dog , ball|dog ball")]
    [InlineData("""[Prop(Pet), Alt(",", "and"), Prop(Toy)]""", "dog, ball|dog and ball", "dog ball|dogand ball")]
    [InlineData("""[Prop(Pet), Alt(", and", " and"), Prop(Toy)]""", "dog, and ball|dog and ball", "dog  and ball")]
    [InlineData("""[Prop(Pet), Some(",", "and"), Prop(Toy)]""", "dog, ball|dog and ball|dog, and ball", "dog ball")]
    [InlineData("""[Prop(Pet), Opt(",", "and"), Prop(Toy)]""", "dog, ball|dog and ball|dog ball", "dog , ball")]
    [InlineData("""["the", Prop(Pet), "eats", Opt("the "), Prop(Toy)]""", "the dog eats the ball|the dog eats ball", "the dog eatsball")]
    [InlineData("""["(", Prop(Pet), ")"]""", "(dog)", "( dog )|(dog )")]
    [InlineData("""["costs", "$", Prop(Count)]""", "costs $3", "costs $ 3|costs$3")]
    [InlineData("""[Prop(Pet), "-", Prop(Toy)]""", "dog-ball", "dog - ball|dog ball")]
    [InlineData("""[Prop(Pet), " - ", Prop(Toy)]""", "dog - ball", "dog-ball|dog  -  ball")]
    [InlineData("""[Prop(Pet), "/", Prop(Toy)]""", "dog/ball", "dog / ball")]
    [InlineData("""[Prop(Count), "%", Prop(Pet)]""", "50% dog", "50 % dog|50%dog")]
    [InlineData("""["the", Pattern("big|small"), Prop(Pet)]""", "the big dog|the small dog", "small dog|the dog")]
    [InlineData("""["the", Pattern("(big)?"), Prop(Pet)]""", "the big dog|the dog", "thebig dog|the bigdog")]
    [InlineData("""["THE", Alt("Big", "Small"), Pattern("Do+g"), Prop(Toy)]""", "the big dog ball|the small dooog ball", "the dog ball")]
    [InlineData("""["feed the", Plural("berry"), "to the", Prop(Pet)]""", "feed the berry to the dog|feed the berries to the dog", "feed the berrys to the dog")]
    [InlineData("""["the", Prop(Pet), Opt("'s"), "bowl"]""", "the dog's bowl|the dog bowl", "the dog 's bowl")]
    public void Spacing_is_the_engines_unless_the_author_writes_it(string nibs, string matching, string notMatching)
    {
        var grammar = Build(nibs);

        Assert.All(matching.Split('|'), x => Assert.True(Matches(grammar, x), $"{nibs} should match \"{x}\""));
        Assert.All(notMatching.Split('|'), x => Assert.False(Matches(grammar, x), $"{nibs} shouldn't match \"{x}\""));
    }
}
