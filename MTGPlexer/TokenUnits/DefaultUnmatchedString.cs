namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class DefaultUnmatchedString : TokenUnit
{
    protected override string[] Snippets => [@"[^\s]+"];
}