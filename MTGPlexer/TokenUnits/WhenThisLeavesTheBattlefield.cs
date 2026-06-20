namespace MTGPlexer.TokenUnits;

public class WhenThisLeavesTheBattlefield : TokenUnit
{
    public override Snippet[] Snippets => ["when {this} leaves the battlefield,", Prop(Result)];

    public DynamicToken Result { get; set; }
}