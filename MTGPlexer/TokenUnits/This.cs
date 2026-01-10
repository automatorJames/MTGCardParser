namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
[Dependent]
public class This : TokenUnit
{
    protected override string[] Snippets => [@"{this}"];

}