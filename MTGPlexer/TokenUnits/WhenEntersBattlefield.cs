namespace MTGPlexer.TokenUnits;

public class WhenEntersBattlefield : TokenUnit
{
    protected override Snippet[] Snippets => ["target", Prop(CardType)];

    public CardType CardType { get; set; }
}