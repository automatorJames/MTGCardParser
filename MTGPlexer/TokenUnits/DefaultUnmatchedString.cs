namespace MTGPlexer.TokenUnits;

[IgnoreInAnalysis]
[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
public class DefaultUnmatchedString : TokenUnit
{
    public DefaultUnmatchedString() : base(@"[^\s]+") { }

}

