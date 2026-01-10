namespace MTGPlexer.TokenUnits;

public class CardOutsideBattlefield() : TokenUnit
{
    protected override string[] Snippets => ["(card|spell)", "((in|from) )?", nameof(Whose), nameof(Zone)];

    public Whose? Whose { get; set; }
    public NonBattlefieldZone Zone { get; set; }
}

 