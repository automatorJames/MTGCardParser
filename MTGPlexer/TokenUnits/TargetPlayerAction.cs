namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class TargetPlayerAction : TokenUnit
{
    protected override string[] Snippets => ["target", nameof(PlayerIdentity), nameof(Action), @"\."];

    public PlayerIdentity PlayerIdentity { get; set; }
    public DynamicCapture<TokenUnit> Action { get; set; }
}