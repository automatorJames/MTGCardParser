namespace MTGPlexer.TokenUnits;

public class HasAbility : TokenUnit
{
    public override Snippet[] Snippets => ["has \"", Prop(Ability), "\""];
    public DynamicToken Ability { get; set; }
}