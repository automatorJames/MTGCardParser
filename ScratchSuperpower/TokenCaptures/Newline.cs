namespace MTGCardParser.TokenCaptures;

public class Newline : ITokenCapture
{
    public string RegexTemplate => @"\n";
}