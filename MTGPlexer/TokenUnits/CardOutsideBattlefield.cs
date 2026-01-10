namespace MTGPlexer.TokenUnits;

public class CardOutsideBattlefield() : TokenUnit
{
    protected override Snippet[] Snippets => ["(card|spell)", "((in|from) )?", Prop(Whose), Prop(Zone)];

    public Whose? Whose { get; set; }
    public NonBattlefieldZone Zone { get; set; }
}

 