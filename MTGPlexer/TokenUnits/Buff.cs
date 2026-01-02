namespace MTGPlexer.TokenUnits;

[Dependent]
public class Buff() : TokenUnitOneOf
{
    public TransformedType TransformedType { get; set; }
    public PowerToughnessMod PowerToughnessModification { get; set; }
    public Keyword Keyword { get; set; }
}