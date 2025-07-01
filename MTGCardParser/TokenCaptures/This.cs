namespace MTGCardParser.TokenCaptures;

public class This : ITokenUnit
{
    public RegexTemplate<This> RegexTemplate => new(@"\{this\}");
}