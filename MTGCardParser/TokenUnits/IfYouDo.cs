namespace MTGCardParser.TokenUnits;

public class IfYouDo : TokenUnit
{
    public RegexTemplate<IfYouDo> RegexTemplate => new("if you do,");
}