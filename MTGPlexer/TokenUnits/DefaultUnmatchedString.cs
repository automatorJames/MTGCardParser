namespace MTGPlexer.TokenUnits;

[IgnoreInAnalysis]
public class DefaultUnmatchedString : TokenUnit
{
    public DefaultUnmatchedString() : base(@"[^\s]+") { }

}

