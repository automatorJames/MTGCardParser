namespace MTGPlexer.TokenUnits;

[IgnoreInAnalysis]
[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
public class DefaultUnmatchedString : TokenUnit
{
    protected override string[] Snippets => [@"[^\s]+"];
}