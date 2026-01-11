namespace MTGPlexer.TokenUnits;

public class ThisDealsDamage : TokenUnit
{
    protected override Snippet[] Snippets => ["{this} deals", Prop(Quantity), "damage to", Prop(Recipient)];

    public Quantity Quantity { get; set; }
    public Recipient Recipient { get; set; }
}
