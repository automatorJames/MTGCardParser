namespace MTGCardParser.TokenUnits;

public class This : TokenUnit
{
    public RegexTemplate RegexTemplate => new(@"\{this\}");
}