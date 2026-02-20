namespace MTGPlexer.TokenUnitPrimitives.Internal;

public class ManyOfSecondItem<T> : TokenUnit
{
    public override Snippet[] Snippets => [",[ ]", Prop(Item)];
    public override Joiner Joiner => Joiner.None;

    public T Item { get; set; }
}
