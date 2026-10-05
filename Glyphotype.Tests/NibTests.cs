namespace Glyphotype.Tests;

/// <summary>A string nib is literal text; regex is an explicit opt-in (<see cref="PatternNib"/>).</summary>
public class NibTests
{
    [Theory]
    [InlineData("plain words", "plain words")]
    [InlineData("$3", @"\$3")]
    [InlineData("{this}", @"\{this}")]
    [InlineData("2+2?", @"2\+2\?")]
    [InlineData("a.b", @"a\.b")]
    [InlineData("[ctrl]", @"\[ctrl]")]
    [InlineData(@"c:\path", @"c:\\path")]
    [InlineData("(an)|(a)^*", @"\(an\)\|\(a\)\^\*")]
    public void A_string_nib_matches_its_text_literally(string text, string expectedRegex)
    {
        Nib nib = text;

        Assert.Equal(text, nib.Text);
        Assert.Equal(expectedRegex, nib.Regex);
        Assert.Matches($"^(?:{nib.Regex})$", text);
    }

    [Fact]
    public void A_pattern_nib_is_used_as_written()
    {
        var nib = new PatternNib("an?");

        Assert.Equal("an?", nib.Regex);
        Assert.Matches($"^(?:{nib.Regex})$", "a");
        Assert.Matches($"^(?:{nib.Regex})$", "an");
    }

    [Fact]
    public void The_text_helpers_take_literal_text()
    {
        Assert.Equal(@"(\$|\?)", new NibAlternatives("$", "?").Regex);
        Assert.Equal(@"\$", new OptionalNib("$").Regex);
        Assert.Equal(@"(in|from)", new OptionalNib("in", "from").Regex);
    }

    [Theory]
    [InlineData("creature", "creature|creatures", "creatur|creaturess")]
    [InlineData("berry", "berry|berries", "berrys|berr")]
    [InlineData("box", "box|boxes", "boxs")]
    [InlineData("enchanted creature", "enchanted creature|enchanted creatures", "enchanteds creature")]
    public void Plural_matches_the_word_singular_or_plural(string singular, string matching, string notMatching)
    {
        var nib = new PluralNib(singular);

        Assert.All(matching.Split('|'), x => Assert.Matches($"^(?:{nib.Regex})$", x));
        Assert.All(notMatching.Split('|'), x => Assert.DoesNotMatch($"^(?:{nib.Regex})$", x));
    }

    [Fact]
    public void A_pattern_that_can_match_nothing_is_optional()
    {
        Assert.True(new PatternNib("(an?)?").IsOptional);
        Assert.True(new PatternNib(@"\d*").IsOptional);
        Assert.False(new PatternNib("an?").IsOptional);
    }

    [Fact]
    public void The_text_helpers_refuse_empty_and_pointless_texts()
    {
        Assert.Throws<ArgumentException>(() => new NibAlternatives("", "big"));
        Assert.Throws<ArgumentException>(() => new NibAlternatives("big"));
        Assert.Throws<ArgumentException>(() => new SomeNib("big"));
        Assert.Throws<ArgumentException>(() => new OptionalNib());
        Assert.Throws<ArgumentException>(() => new OptionalNib("a", ""));
        Assert.Throws<ArgumentException>(() => new PluralNib(""));
    }
}
