namespace MTGPlexer.TokenUnits;

public class ThatCardsController : TokenUnit
{
    protected override Snippet[] Snippets => ["that", Prop(CardOrCreatureType), "'s controller"];

    public OneOf<CardType, CreatureType> CardOrCreatureType { get; set; }
}
