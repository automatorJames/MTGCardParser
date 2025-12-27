namespace MTGPlexer.TokenUnits;

[Dependent]
public class Buff() : TokenUnitOneOf
{
    public PowerToughnessMod PowerToughnessModification { get; set; }
    public Keyword Keyword { get; set; }
}