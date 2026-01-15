namespace MTGPlexer.TokenUnits;

public class WhenThisLeavesTheBattlefield : TokenUnit
{
    protected override Snippet[] Snippets => ["when {this} leaves the battlefield,", Prop(Result)];

    public DynamicOf<TokenUnit> Result { get; set; }
}