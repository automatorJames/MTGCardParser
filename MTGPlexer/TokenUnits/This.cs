namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
[TokenUnitProperty]
public class This : TokenUnit
{
    public This() : base(@"\{this\}") { }
}