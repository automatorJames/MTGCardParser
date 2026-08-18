namespace MTGGlyphs;

[Dependent]
public class TargetableEntity : GlyphOneOf
{
    public TargetablePlayer? TargetablePlayer { get; set; }
    public CardType? CardType { get; set; }
    public CreatureType? CreatureType { get; set; }
}

public enum TargetablePlayer
{
    Player,
    Opponent
}

