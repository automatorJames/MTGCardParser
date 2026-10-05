using Glyphotype.Definitions;
using Glyphotype.Distiller.Inspection;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Tests;

/// <summary>
/// Glyphs that match an enclosure (see <see cref="Enclosures"/>) themselves: reminder text in parentheses, an ability
/// in quotes - whose periods end clauses nested in the glyph's own text, not clauses of the line.
/// </summary>
public class EnclosureTests
{
    static GlyphGrammar Build(string source)
    {
        var read = GlyphSourceReader.Read(source);
        return GlyphGrammar.FromDefinition(new GrammarDefinition { Glyphs = read.Glyphs, Vocabularies = read.Vocabularies, Markers = read.Markers }, allowPartialClauseMatches: false);
    }

    static string Render(GlyphGrammar grammar, string text) =>
        ParseRenderer.Render(grammar.Tokenize(text), nested: true);

    const string _reminders = """
        public class Flying : Glyph
        {
            public override Nib[] Nibs => ["flying"];
        }

        public class ReminderText : Glyph
        {
            public override Nib[] Nibs => ["(", Prop(Reminder), Opt("."), ")"];

            [AllowUnmatched]
            public DynamicGlyph Reminder { get; set; }
        }
        """;

    [Theory]
    [InlineData("flying (this creature can't be blocked.)", "this creature can't be blocked")]
    [InlineData("flying (this creature can't be blocked)", "this creature can't be blocked")]
    [InlineData("flying (it flies. it can't be blocked.)", "it flies. it can't be blocked")]
    public void A_glyph_takes_a_parenthetical_whole_leaving_the_closing_period_to_its_own_nib(string text, string reminder)
    {
        var grammar = Build(_reminders);

        Assert.Equal($"⟦Flying: flying⟧ ⟦ReminderText: (⟦Reminder: «{reminder}»⟧{(text.EndsWith(".)") ? "." : "")})⟧", Render(grammar, text));
    }

    [Fact]
    public void Without_a_nib_for_the_closing_period_the_enclosed_text_keeps_it()
    {
        var grammar = Build("""
            public class Flying : Glyph
            {
                public override Nib[] Nibs => ["flying"];
            }

            public class ReminderText : Glyph
            {
                public override Nib[] Nibs => ["(", Prop(Reminder), ")"];

                [AllowUnmatched]
                public DynamicGlyph Reminder { get; set; }
            }
            """);

        Assert.Equal("⟦Flying: flying⟧ ⟦ReminderText: (⟦Reminder: «it flies.»⟧)⟧", Render(grammar, "flying (it flies.)"));
    }

    [Fact]
    public void A_glyph_spans_a_quoted_ability_and_resolves_what_it_quotes()
    {
        var grammar = Build("""
            public class Flies : Glyph
            {
                public override Nib[] Nibs => ["it flies"];
            }

            public class HasAbility : Glyph
            {
                public override Nib[] Nibs => ["the bird has", "\"", Prop(Ability), Opt("."), "\""];
                public DynamicGlyph Ability { get; set; }
            }
            """);

        Assert.Equal("⟦HasAbility: the bird has \"⟦Ability→Flies: it flies⟧.\"⟧ .", Render(grammar, "the bird has \"it flies.\"."));
    }

    [Fact]
    public void A_period_written_inside_the_glyphs_own_parentheses_is_no_clause_break()
    {
        // Literal text with an enclosed period, and an Opt(".") inside parentheses: both valid, both matching.
        var grammar = Build("""
            public class Flying : Glyph
            {
                public override Nib[] Nibs => ["flying (it flies. it", Opt("really."), "can't be blocked.)"];
            }
            """);

        Assert.Empty(grammar.GetStructuralValidationErrors());
        Assert.Equal("⟦Flying: flying (it flies. it can't be blocked.)⟧", Render(grammar, "flying (it flies. it can't be blocked.)"));
        Assert.Equal("⟦Flying: flying (it flies. it really. can't be blocked.)⟧", Render(grammar, "flying (it flies. it really. can't be blocked.)"));
    }

    [Fact]
    public void An_enclosure_no_glyph_takes_is_tokenized_inside_as_clauses_of_its_own()
    {
        var grammar = Build(_reminders.Replace("ReminderText", "Unused").Replace("""["(", Prop(Reminder), Opt("."), ")"]""", """["[", Prop(Reminder), "]"]"""));
        var tokens = grammar.Tokenize("flying (flying. flying.)");

        Assert.Equal("⟦Flying: flying⟧ ( ⟦Flying: flying⟧ . ⟦Flying: flying⟧ . )", ParseRenderer.Render(tokens, nested: true));
        Assert.All(tokens.OfType<ClauseBreak>(), x => Assert.Equal(1, x.Depth));
        Assert.All(tokens.OfType<EnclosureMark>(), x => Assert.Equal(0, x.Depth));
    }
}
