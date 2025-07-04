namespace MTGCardParser.TokenUnits;

public class IfYouDo : TokenUnitBase
{
    public RegexTemplate<IfYouDo> RegexTemplate => new("if you do,");
}