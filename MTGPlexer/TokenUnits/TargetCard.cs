namespace MTGPlexer.TokenUnits;

public class TargetCard : TokenUnit
{
    public override Snippet[] Snippets => ["target", Prop(CardType)];

    public CardType CardType { get; set; }
}