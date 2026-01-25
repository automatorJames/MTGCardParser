namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class HasAbility : TokenUnit
{
    protected override Snippet[] Snippets => ["has \"", Prop(Ability), "\""];
    public DynamicOf<TokenUnit> Ability { get; set; }
}