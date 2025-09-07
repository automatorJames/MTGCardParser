namespace MTGPlexer.TokenUnits;

[NoBoundary]
[TokenUnitProperty]
public class This : TokenUnit
{
    public This() : base(@"\{this\}") { }
}