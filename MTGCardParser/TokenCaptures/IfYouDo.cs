namespace MTGCardParser.TokenCaptures;

public class IfYouDo : ITokenCapture
{
    public RegexTemplate<IfYouDo> RegexTemplate => new("if you do,");
}