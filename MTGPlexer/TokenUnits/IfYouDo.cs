namespace MTGPlexer.TokenUnits;

public class IfYouDo : TokenUnit
{
    protected override Snippet[] Snippets => ["if you do,", Prop(Outcome)];

    public DynamicOf<TokenUnit> Outcome { get; set; }
}