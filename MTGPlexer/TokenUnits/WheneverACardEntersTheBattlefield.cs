namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class WheneverACardEntersTheBattlefield : TokenUnit
{
    public override Snippet[] Snippets => ["whenever a", Prop(CardOrCreatureType), "enters the battlefield,", Prop(Result)];

    public OneOf<CardType, CreatureType> CardOrCreatureType { get; set; }
    public DynamicToken Result { get; set; }
}
