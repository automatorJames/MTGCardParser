namespace MTGGlyphs;

public class ThatCardsController : Glyph
{
    public override Nib[] Nibs => ["that", Prop(CardOrCreatureType), "'s controller"];

    public OneOf<CardType, CreatureType> CardOrCreatureType { get; set; }
}
