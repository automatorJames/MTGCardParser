namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a target can be: a player, a card type or a creature type.</summary>
[Dependent]
public class AnyTarget : GlyphOneOf
{
    public PlayerIdentity? PlayerIdentity { get; set; }
    public CardType? CardType { get; set; }
    public CreatureType? CreatureType { get; set; }
}