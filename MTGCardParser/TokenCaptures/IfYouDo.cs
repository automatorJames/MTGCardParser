namespace MTGCardParser.TokenCaptures;

public class IfYouDo : ITokenUnit
{
    public RegexTemplate<IfYouDo> RegexTemplate => new("if you do,");
}