namespace MTGCardParser.TokenUnits;

[IgnoreInAnalysis]
public class DefaultUnmatchedString : TokenUnit
{
    public RegexTemplate RegexTemplate => new(@"[^.,;""\s]+");
}

