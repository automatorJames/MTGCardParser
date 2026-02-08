namespace MTGPlexer.TokenUnits;

public class HasAbility : TokenUnit
{
    protected override Snippet[] Snippets => ["has \"", Prop(Ability), "\""];
    public DynamicOf<TokenUnit> Ability { get; set; }
}