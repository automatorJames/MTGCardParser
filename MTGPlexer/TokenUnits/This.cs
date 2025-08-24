namespace MTGPlexer.TokenUnits;

[NoWordBoundary]
public class This : TokenUnitProperty
{
    public This() : base(@"\{this\}") { }
}