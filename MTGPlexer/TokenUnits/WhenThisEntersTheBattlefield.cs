namespace MTGPlexer.TokenUnits;

public class WhenThisEntersTheBattlefield : TokenUnit
{
    public override Snippet[] Snippets => ["when {this} enters the battlefield,", Prop(MustStillBeOnTheBattlefield), "it", Prop(GainedOrLostAbilities)];

    [RegexPattern("if it's on the battlefield,")]
    public bool MustStillBeOnTheBattlefield { get; set; }

    public ManyOf<GainOrLoseAbility> GainedOrLostAbilities { get; set; }
}