namespace MTGCardParser.TokenCaptures;

public class Period : ITokenCapture
{
    public string RegexTemplate => @"\.";
}