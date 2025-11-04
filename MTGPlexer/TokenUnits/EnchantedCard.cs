namespace MTGPlexer.TokenUnits;

public class EnchantedCard : TokenUnit
{
    protected override string[] Snippets => ["enchanted", nameof(CardType), nameof(PermanentVerb)];

    public CardType CardType { get; set; }
    public PermanentVerb? PermanentVerb { get; set; }
}