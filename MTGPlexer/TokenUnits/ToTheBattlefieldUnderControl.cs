namespace MTGPlexer.TokenUnits;

public class ToTheBattlefieldUnderControl : TokenUnit
{
    protected override Snippet[] Snippets => ["(on)?to the battlefield under", Prop(Whose), "control", Prop(AndAttachThisToIt)];

    public Whose Whose { get; set; }

    [RegexPattern("and attach {this} to it")]
    public bool AndAttachThisToIt { get; set; }
}