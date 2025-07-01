namespace MTGCardParser.TokenCaptures;

[NoSpaces]
public class Parenthetical : ITokenUnit
{
    public RegexTemplate<Parenthetical> RegexTemplate => new(@"\(", nameof(Content), @"\)");

    [RegexPattern(@"([^)]*)")]
    public CapturedTextSegment Content { get; set; }
}