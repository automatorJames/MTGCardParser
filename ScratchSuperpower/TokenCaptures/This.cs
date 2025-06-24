namespace MTGCardParser.TokenCaptures;

public class This : ITokenCapture
{
    public string RegexTemplate => @"\{this\}";
}