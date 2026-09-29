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
    public void Opt_and_Alt_take_literal_text_and_Opt_also_a_pattern()
    {
        Assert.Equal(@"(\$|\?)", new NibAlternatives("$", "?").Regex);
        Assert.Equal(@"\$", new OptionalNib("$").Regex);
        Assert.Equal("an?", new OptionalNib(new PatternNib("an?")).Regex);
    }
}
