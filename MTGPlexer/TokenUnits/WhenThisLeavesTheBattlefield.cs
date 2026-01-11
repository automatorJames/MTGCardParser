namespace MTGPlexer.TokenUnits;

public class WhenThisLeavesTheBattlefield : TokenUnit
{
    protected override Snippet[] Snippets => ["when {this} leaves the battlefield,", Prop(Result)];

    public DynamicCapture<TokenUnit> Result { get; set; }
}