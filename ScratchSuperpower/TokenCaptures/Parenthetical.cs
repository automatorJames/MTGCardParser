namespace MTGCardParser.TokenCaptures;

public class Parenthetical : ITokenCapture
{
    public string RegexTemplate => @"\(([^)]*)\)";

    public TokenSegment Content { get; set; }
}