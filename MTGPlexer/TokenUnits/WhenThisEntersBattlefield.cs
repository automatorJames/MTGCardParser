namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class WhenThisEntersBattlefield : TokenUnit
{
    protected override Snippet[] Snippets => ["when {this} enters the battlefield,", Prop(MustStillBeOnTheBattlefield), "it", Prop(GainedOrLostAbilities)];

    [RegexPattern("if it's on the battlefield,")]
    public bool MustStillBeOnTheBattlefield { get; set; }

    public ManyOf<GainOrLoseAbility> GainedOrLostAbilities { get; set; }

    //public DynamicCapture<TokenUnit> Effect { get; set; }
}