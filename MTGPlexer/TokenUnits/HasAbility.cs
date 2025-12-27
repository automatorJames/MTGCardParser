namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class HasAbility : TokenUnit
{
    protected override string[] Snippets => ["has \"", nameof(Ability), "\""];
    public DynamicCapture<TokenUnit> Ability { get; set; }
}
