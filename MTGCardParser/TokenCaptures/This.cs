namespace MTGCardParser.TokenCaptures;

public class This : ITokenCapture
{
    public RegexTemplate<This> RegexTemplate => new(@"\{this\}");
}