namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class HasAbility : TokenUnit
{
    protected override Snippet[] Snippets => ["has \"", Prop(Ability), "\""];
    public DynamicCapture<TokenUnit> Ability { get; set; }
}
