namespace MTGCardParser.TokenUnits;

[IgnoreInAnalysis]
public class DefaultUnmatchedString : TokenUnit
{
    public RegexTemplate<Cost> RegexTemplate => new(@"[^.,;""\s]+");
}

