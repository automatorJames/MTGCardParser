namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
[Dependent]
public class This : TokenUnit
{
    protected override string[] Snippets => [@"{this}"];

}