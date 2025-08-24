namespace MTGPlexer.TokenUnits;

[IgnoreInAnalysis]
[NoWordBoundary]
public class DefaultUnmatchedString : TokenUnit
{
    public DefaultUnmatchedString() : base(@"[^\s]+") { }

}

