namespace MTGPlexer.TokenUnits;

public class WheneverACardEntersTheBattlefield : TokenUnit
{
    protected override Snippet[] Snippets => ["whenever a", Prop(CardOrCreatureType), "enters the battlefield,", Prop(Result)];

    public OneOf<CardType, CreatureType> CardOrCreatureType { get; set; }
    public DynamicCapture<TokenUnit> Result { get; set; }

}
