namespace MTGCardParser.TokenCaptures;

public class Parenthetical : ITokenCapture
{
    public string RegexTemplate => $@"\((?<{nameof(Content)}>)([^)]*)\)";

    public TokenSegment Content { get; set; }
}