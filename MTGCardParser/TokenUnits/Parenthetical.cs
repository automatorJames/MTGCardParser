namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class Parenthetical : TokenUnit
{
    public RegexTemplate<Parenthetical> RegexTemplate => new(@"\(", nameof(Content), @"\)");

    [RegexPattern(@"([^)]*)")]
    public PlaceholderCapture Content { get; set; }
}