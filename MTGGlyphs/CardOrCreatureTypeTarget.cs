namespace MTGGlyphs;

[Dependent]
public class CardOrCreatureTypeTarget : GlyphOneOf
{
    public CardType CardType { get; set; }
    public CreatureType CreatureType { get; set; }
}