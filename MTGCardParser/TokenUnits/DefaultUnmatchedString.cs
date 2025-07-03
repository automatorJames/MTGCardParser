namespace MTGCardParser.TokenUnits;

[IgnoreInAnalysis]
public class DefaultUnmatchedString : ITokenUnit
{
    public RegexTemplate<Cost> RegexTemplate => new(@"[^.,;""\s]+");
}

