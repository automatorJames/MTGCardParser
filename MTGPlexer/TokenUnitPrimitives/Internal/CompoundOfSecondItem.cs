namespace MTGPlexer.TokenUnitPrimitives.Internal;

public class CompoundOfSecondItem<T> : TokenUnit
{
    protected override Snippet[] Snippets => ["[ ]", Prop(Item)];

    public T Item { get; set; }
}
