namespace MTGPlexer.TokenUnits;

public class WhenThisEntersBattlefield : TokenUnit
{
    protected override Snippet[] Snippets => ["when {this} enters the battlefield,", Prop(MustStillBeOnTheBattlefield)];

    [RegexPattern("if it's on the battlefield,")]
    public bool MustStillBeOnTheBattlefield { get; set; }

    //public DynamicCapture<TokenUnit> Effect { get; set; }
}