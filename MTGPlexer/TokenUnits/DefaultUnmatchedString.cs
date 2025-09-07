namespace MTGPlexer.TokenUnits;

[IgnoreInAnalysis]
[NoBoundary]
public class DefaultUnmatchedString : TokenUnit
{
    public DefaultUnmatchedString() : base(@"[^\s]+") { }

}

