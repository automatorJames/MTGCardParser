namespace MTGPlexer.TokenUnits;

public class TargetPlayerAction : TokenUnit
{
    protected override Snippet[] Snippets => ["target", Prop(PlayerIdentity), Prop(Action)];

    public PlayerIdentity PlayerIdentity { get; set; }
    public DynamicOf<TokenUnit> Action { get; set; }
}