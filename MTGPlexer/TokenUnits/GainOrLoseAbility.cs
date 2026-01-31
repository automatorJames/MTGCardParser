namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class GainOrLoseAbility : TokenUnit
{
    protected override Snippet[] Snippets => [Prop(GainOrLose), "\"", Prop(Ability), "\""];

    public GainOrLose GainOrLose { get; set; }
    
    [RegexPattern("[^\"]+")]
    public DynamicOf<TokenUnit> Ability { get; set; }
}