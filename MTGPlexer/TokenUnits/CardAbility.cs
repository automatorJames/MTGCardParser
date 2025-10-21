namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.FullLine)]
public class CardAbility : TokenUnit
{
    //public ManyOf<Keyword> Keywords { get; set; }
    [OptionalMany]
    public Keyword Keyword { get; set; }
}