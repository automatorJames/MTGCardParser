namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class IfYouDo : TokenUnit
{
    public override Snippet[] Snippets => ["if you do,", Prop(Outcome)];

    public DynamicOf<TokenUnit> Outcome { get; set; }
}