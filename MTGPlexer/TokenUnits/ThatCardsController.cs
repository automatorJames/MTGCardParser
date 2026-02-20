namespace MTGPlexer.TokenUnits;

public class ThatCardsController : TokenUnit
{
    public override Snippet[] Snippets => ["that", Prop(CardOrCreatureType), "'s controller"];

    public OneOf<CardType, CreatureType> CardOrCreatureType { get; set; }
}
