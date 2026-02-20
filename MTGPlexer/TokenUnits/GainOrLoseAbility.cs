namespace MTGPlexer.TokenUnits;

public class GainOrLoseAbility : TokenUnit
{
    public override Snippet[] Snippets => [Prop(GainOrLose), "\"", Prop(Ability), "\""];

    public GainOrLose GainOrLose { get; set; }
    
    [RegexPattern("[^\"]+")]
    public DynamicOf<TokenUnit> Ability { get; set; }
}