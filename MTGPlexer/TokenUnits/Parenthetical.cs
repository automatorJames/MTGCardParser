namespace MTGPlexer.TokenUnits;

[NoSpaces]
[Color("#666666")]
public class Parenthetical : TokenUnit
{
    public Parenthetical() : base(@"\(", nameof(Content), @"\)") { }

    [RegexPattern(@"([^)]*)")]
    public PlaceholderCapture Content { get; set; }
}