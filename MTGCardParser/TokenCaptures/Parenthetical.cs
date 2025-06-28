namespace MTGCardParser.TokenCaptures;

public class Parenthetical : TokenCaptureBase<Parenthetical>
{
    public override RegexTemplate<Parenthetical> RegexTemplate => new(@"\(", nameof(Content), @"\)");

    [RegexPattern(@"([^)]*)")]
    public TokenSegment Content { get; set; }
}