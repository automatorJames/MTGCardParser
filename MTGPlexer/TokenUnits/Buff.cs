namespace MTGPlexer.TokenUnits;

[Dependent]
public class Buff() : TokenUnitOneOf
{
    public PowerToughnessMod PowerToughnessMod { get; set; }
    public Keyword Keyword { get; set; }
}