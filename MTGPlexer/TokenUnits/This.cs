namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
[Dependent]
public class This : TokenUnit
{
    public override Snippet[] Snippets => [@"{this}"];

}