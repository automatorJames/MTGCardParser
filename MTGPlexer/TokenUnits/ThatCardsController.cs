namespace MTGPlexer.TokenUnits;

public class ThatCardsController : TokenUnit
{
    protected override Snippet[] Snippets => ["that", Prop(CardOrCreatureType), "'s controller", Prop(SacrificeIt)];

    public OneOf<CardType, CreatureType> CardOrCreatureType { get; set; }

    public OptionalOf<SacrificeIt> SacrificeIt { get; set; }
}
