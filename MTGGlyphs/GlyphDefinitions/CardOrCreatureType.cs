namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card type or a creature type: "artifact", "creature", "wall".</summary>
[Dependent]
public class CardOrCreatureType : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public CreatureType? CreatureType { get; set; }
}
