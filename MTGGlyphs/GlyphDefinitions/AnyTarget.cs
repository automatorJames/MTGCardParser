namespace MTGGlyphs.GlyphDefinitions;

[Dependent]
public class AnyTarget : GlyphOneOf
{
    public PlayerIdentity? PlayerIdentity { get; set; }
    public CardType? CardType { get; set; }
    public CreatureType? CreatureType { get; set; }
}