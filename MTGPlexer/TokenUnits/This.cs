namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
[TokenUnitProperty]
public class This : TokenUnit
{
    protected override string[] Snippets => [@"{this}"];

}