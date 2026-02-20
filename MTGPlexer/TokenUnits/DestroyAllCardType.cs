namespace MTGPlexer.TokenUnits;

public class DestroyAllCardType : TokenUnit
{
    public override Snippet[] Snippets => ["destroy all", Prop(CardType, Proptions.Plural)];

    public CardType CardType { get; set; }
}