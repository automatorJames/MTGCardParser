namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.FullLine)]
public class CardAbility : TokenUnit
{
    [OptionalMany]
    public Keyword Keyword { get; set; }
}