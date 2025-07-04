namespace MTGCardParser.TokenUnits;

[IgnoreInAnalysis]
public class DefaultUnmatchedString : TokenUnitBase
{
    public RegexTemplate<Cost> RegexTemplate => new(@"[^.,;""\s]+");
}

