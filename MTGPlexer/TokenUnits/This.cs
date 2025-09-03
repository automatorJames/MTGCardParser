namespace MTGPlexer.TokenUnits;

[NoWordBoundary]
[TokenUnitProperty]
public class This : TokenUnit
{
    public This() : base(@"\{this\}") { }
}