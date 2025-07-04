namespace MTGCardParser.TokenUnits;

public class This : TokenUnitBase
{
    public RegexTemplate<This> RegexTemplate => new(@"\{this\}");
}