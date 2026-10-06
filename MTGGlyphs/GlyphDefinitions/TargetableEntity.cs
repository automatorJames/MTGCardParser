namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What can be targeted: a player or opponent, a card type or a creature type.</summary>
/// <exampledoc>Ancestral Recall</exampledoc>
/// <examplecapture>player</examplecapture>
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

