namespace MTGCardParser.TokenCaptures;

public class IfYouDo : ITokenCapture
{
    public string RegexTemplate => $@"if you do,";
}