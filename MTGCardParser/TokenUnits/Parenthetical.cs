namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class Parenthetical : TokenUnit
{
    public RegexTemplate RegexTemplate => new(@"\(", nameof(Content), @"\)");

    [RegexPattern(@"([^)]*)")]
    public PlaceholderCapture Content { get; set; }
}