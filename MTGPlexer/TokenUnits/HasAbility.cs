namespace MTGPlexer.TokenUnits;

public class HasAbility : TokenUnit
{
    public override Snippet[] Snippets => ["has \"", Prop(Ability), "\""];
    public DynamicOf<TokenUnit> Ability { get; set; }
}