namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
public class DefaultUnmatchedString : TokenUnit
{
    protected override Snippet[] Snippets => [@"[^\s]+"];
}