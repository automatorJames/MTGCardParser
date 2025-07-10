namespace MTGCardParser.TokenUnits;

public class IfYouDo : TokenUnit
{
    public RegexTemplate RegexTemplate => new("if you do,");
}