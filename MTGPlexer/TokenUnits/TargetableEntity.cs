namespace MTGPlexer.TokenUnits;

[Dependent]
public class TargetableEntity : TokenUnitOneOf
{
    public override Snippet[] Snippets => ["target"];

    public TargetablePlayer? TargetablePlayer { get; set; }
    public CardType? CardType { get; set; }
    public CreatureType? CreatureType { get; set; }
}

public enum TargetablePlayer
{
    Player,
    Opponent
}

