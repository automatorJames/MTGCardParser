namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
[Dependent]
public class This : TokenUnit
{
    protected override Snippet[] Snippets => [@"{this}"];

}