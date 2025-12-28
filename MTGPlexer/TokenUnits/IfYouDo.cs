namespace MTGPlexer.TokenUnits;

public class IfYouDo : TokenUnit
{
    protected override string[] Snippets => ["if you do,", nameof(Outcome)];

    public DynamicCapture<TokenUnit> Outcome { get; set; }
}