namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class Parenthetical : TokenUnit
{
    public Parenthetical() : base(@"\(", nameof(Content), @"\)") { }

    [RegexPattern(@"([^)]*)")]
    public PlaceholderCapture Content { get; set; }
}