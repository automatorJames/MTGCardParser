namespace MTGCardParser.TokenUnits;

public class This : TokenUnit
{
    public RegexTemplate<This> RegexTemplate => new(@"\{this\}");
}