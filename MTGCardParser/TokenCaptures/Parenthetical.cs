namespace MTGCardParser.TokenCaptures;

[NoSpaces]
public class Parenthetical : ITokenCapture
{
    public RegexTemplate<Parenthetical> RegexTemplate => new(@"\(", nameof(Content), @"\)");

    [RegexPattern(@"([^)]*)")]
    public TokenSegment Content { get; set; }
}