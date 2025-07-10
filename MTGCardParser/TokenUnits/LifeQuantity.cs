namespace MTGCardParser.TokenUnits;

public class LifeQuantity : TokenUnit
{
    public LifeQuantity() : base(nameof(Quantity), "life") { }

    public Quantity Quantity { get; set; }
}

