namespace MTGPlexer.TokenUnits;

[IsolateForTesting]
public class LifeChangeQuantity : TokenUnit
{
    public override Snippet[] Snippets => [Prop(WhichPlayer), Prop(LifeVerb), Prop(Quantity), "life"];

    public WhichPlayer WhichPlayer { get; set; }
    public LifeVerb LifeVerb { get; set; }
    public Quantity Quantity { get; set; }
}

public enum LifeVerb
{
    Gain,
    Lose
}